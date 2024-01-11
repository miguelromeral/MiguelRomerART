using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.Extensions.Configuration;
using MRA.Services.Firebase.Converters;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using MRA.Web.Models.Art;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase
{
    public class FirestoreService : IFirestoreService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _urlBase;
        private readonly string _collectionNameDrawings;
        private readonly string _collectionNameInspirations;
        private readonly string _collectionNameCollections;
        private readonly DrawingFirebaseConverter _converterDrawing;
        private readonly InspirationFirebaseConverter _converterInspiration;
        private readonly CollectionFirebaseConverter _converterCollection;

        public FirestoreService(string collectionNameDrawing, string collectionNameInspirations, string collectionNameCollections,
            string urlBase, FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _collectionNameDrawings = collectionNameDrawing;
            _collectionNameInspirations = collectionNameInspirations;
            _collectionNameCollections = collectionNameCollections;
            _urlBase = urlBase;
            _converterDrawing = new DrawingFirebaseConverter(_urlBase);
            _converterInspiration = new InspirationFirebaseConverter();
            _converterCollection = new CollectionFirebaseConverter();
        }

        public async Task<List<Drawing>> GetAll()
        {
            var collection = _firestoreDb.Collection(_collectionNameDrawings);
            var documents = (await collection.OrderByDescending("date").GetSnapshotAsync())
                .Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

            return documents.Select(_converterDrawing.ConvertToModel).ToList();
        }


        public async Task<List<Inspiration>> GetAllInspirations()
        {
            try
            {
                var collection = _firestoreDb.Collection(_collectionNameInspirations);
                var snapshot = (await collection.GetSnapshotAsync());
                var inspdocs = snapshot.Documents.Select(s => s.ConvertTo<InspirationDocument>()).ToList();

                return inspdocs.Select(_converterInspiration.ConvertToModel).ToList();
            }catch(Exception ex)
            {
                Console.WriteLine("Error when getting inspirations: " + ex.Message);
                return new List<Inspiration>();
            }
        }


        public async Task<List<Collection>> GetAllCollections()
        {
            try
            {
                var snapshot = (await _firestoreDb.Collection(_collectionNameCollections).GetSnapshotAsync());
                var collections = snapshot.Documents;
                var collectionDocs = collections.Select(s => s.ConvertTo<CollectionDocument>()).ToList();
                var listCollections = new List<Collection>();

                foreach(var collection in collectionDocs)
                {
                    var c = new Collection()
                    {
                        Id = collection.Id,
                        Name = collection.name,
                        Description = collection.description,
                        Drawings = new List<Drawing>(),
                        DrawingsReferences = collection.drawings
                    };

                    var documentosReferenciados = new List<DrawingDocument>();
                    foreach(var reference in collection.drawings)
                    {
                        var documentoReferenciadoSnapshot = await reference.GetSnapshotAsync();
                        if (documentoReferenciadoSnapshot.Exists)
                        {
                            c.Drawings.Add(_converterDrawing.ConvertToModel(documentoReferenciadoSnapshot.ConvertTo<DrawingDocument>()));
                        }
                    }

                    listCollections.Add(c);
                }
                return listCollections;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when getting collections: " + ex.Message);
                return new List<Collection>();
            }
        }

        public async Task<List<Drawing>> Filter(FilterDrawingModel filter)
        {
            try
            {
                Query query = _firestoreDb.Collection(_collectionNameDrawings);

                if (filter.Favorites)
                {
                    query = query.WhereEqualTo("favorite", true);
                }
                if (!String.IsNullOrEmpty(filter.TextQuery))
                {
                    query = query.WhereArrayContainsAny("tags", DeleteAndAdjustTags(filter.Tags));
                }
                if (!String.IsNullOrEmpty(filter.Collection))
                {
                    query = query.WhereArrayContains("collection", filter.Collection);
                }
                if (!String.IsNullOrEmpty(filter.ProductName))
                {
                    if (filter.ProductName.Equals("none"))
                    {
                        query = query.WhereEqualTo("product_name", "");
                    }
                    else
                    {
                        query = query.WhereEqualTo("product_name", filter.ProductName);
                    }
                }
                if (!String.IsNullOrEmpty(filter.CharacterName))
                {
                    if (filter.CharacterName.Equals("none"))
                    {
                        query = query.WhereEqualTo("name", "");
                    }
                    else
                    {
                        query = query.WhereEqualTo("name", filter.CharacterName);
                    }
                }
                if (!String.IsNullOrEmpty(filter.ModelName))
                {
                    if (filter.ModelName.Equals("none"))
                    {
                        query = query.WhereEqualTo("model_name", "");
                    }
                    else
                    {
                        query = query.WhereEqualTo("model_name", filter.ModelName);
                    }
                }
                if (filter.Type > -1)
                {
                    query = query.WhereEqualTo("type", filter.Type);
                }
                if (filter.ProductType > -1)
                {
                    query = query.WhereEqualTo("product_type", filter.ProductType);
                }
                if (filter.Software > 0)
                {
                    query = query.WhereEqualTo("software", filter.Software);
                }
                if (filter.Paper > 0)
                {
                    query = query.WhereEqualTo("paper", filter.Paper);
                }

                var documents = (await query.GetSnapshotAsync()).Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

                return documents.Select(_converterDrawing.ConvertToModel).ToList();
            }catch(Exception ex)
            {
                Debug.WriteLine("Error when filtering documents: " + ex.Message);
            }
            return new List<Drawing>();
        }

        public async Task<Drawing> FindDrawingById(string documentId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(_collectionNameDrawings).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    await UpdateViews(documentId);
                    return _converterDrawing.ConvertToModel(snapshot.ConvertTo<DrawingDocument>());
                }
                 
                return null;
            }catch(Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task<Inspiration> FindInspirationById(string documentId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(_collectionNameInspirations).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var converter = new InspirationFirebaseConverter();
                    return converter.ConvertToModel(snapshot.ConvertTo<InspirationDocument>());
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task<Collection> FindCollectionById(string documentId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection(_collectionNameCollections).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var converter = new CollectionFirebaseConverter();
                    return converter.ConvertToModel(snapshot.ConvertTo<CollectionDocument>());
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }


        public async Task UpdateViews(string documentId)
        {
            // Realiza la transacción para actualizar la propiedad "views"
            await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection(_collectionNameDrawings).Document(documentId);

                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);


                    if (snapshot.ContainsField("views"))
                    {
                        // Si existe, obtiene el valor actual de "views" y le suma uno
                        long currentViews = snapshot.GetValue<long>("views");
                        long newViews = currentViews + 1;

                        // Actualiza la propiedad "views" en el documento
                        transaction.Update(docRef, "views", newViews);
                    }
                    else
                    {
                        // Si no existe, crea la propiedad "views" con el valor inicial de 1
                        transaction.Set(docRef, new { views = 1 }, SetOptions.MergeAll);
                    }

                }catch(Exception ex)
                {
                    Debug.WriteLine("Error when updating views for document '" + documentId + "': " + ex.Message);
                }
            });
        }

        public async Task UpdateLikes(string documentId)
        {
            // Realiza la transacción para actualizar la propiedad "views"
            await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection(_collectionNameDrawings).Document(documentId);

                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);


                    if (snapshot.ContainsField("likes"))
                    {
                        long currentViews = snapshot.GetValue<long>("likes");
                        long newViews = currentViews + 1;

                        transaction.Update(docRef, "likes", newViews);
                    }
                    else
                    {
                        // Si no existe, crea la propiedad "views" con el valor inicial de 1
                        transaction.Set(docRef, new { likes = 1 }, SetOptions.MergeAll);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error when updating views for document '" + documentId + "': " + ex.Message);
                }
            });
        }

        public void SetAutomaticTags(ref Drawing document)
        {
            var list = new List<string>();
            list.AddRange((document.Name ?? "").Split(" ").Select(x => x.ToLower()));
            list.AddRange((document.ModelName ?? "").Split(" ").Select(x => x.ToLower()));
            list.AddRange((document.Title ?? "").Split(" ").Select(x => x.ToLower()));
            if (document.Software > 0)
            {
                list.AddRange(document.SoftwareName.Split(" ").Select(x => x.ToLower()));
            }
            if (document.Paper > 0)
            {
                list.AddRange(document.PaperHuman.Split(" ").Select(x => x.ToLower()));
            }
            if (document.Type > 0)
            {
                list.AddRange(document.TypeName.Split(" ").Select(x => x.ToLower()));
            }

            if (document.ProductType > 0)
            {
                list.AddRange(document.ProductTypeName.Split(" ").Select(x => x.ToLower()));
            }
            list.AddRange(document.ProductName.Split(" ").Select(x => x.ToLower()));

            document.Tags.AddRange(list);
            document.Tags = DeleteAndAdjustTags(document.Tags);
        }

        private List<string> DeleteAndAdjustTags(List<string> tags)
        {
            var eliminar = new List<string>()
            {
                "a",
                "un",
                "unas",
                "unos",
                "uno",
                "de",
                "el",
                "la",
                "los",
                "los",
                "les",
                "the"
            };
            var processed = tags.Select(x => 
                x.ToLower()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace(":", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "")
                .Replace("¿", "")
                .Replace("/", "")
                .Replace("`", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("'", "")
                .Replace("@", " ")
                .Replace("#", " ")
                .Replace("!", "")
                .Replace("¡", "")
                .Replace("~", "")
                .Replace("$", " ")
                .Replace("%", " ")
                .Replace("&", " ")
                .Replace("\"", "")
                .Replace(" ", "")
                .Replace("_", " ")
                .Replace("-", " ")
            );

            var final = new List<string>() { };
            foreach(var s in processed)
            {
                foreach(var s2 in s.Split(" "))
                {
                    if (!eliminar.Contains(s2) && !String.IsNullOrEmpty(s2) && !s2.Equals(" "))
                    {
                        final.Add(s);
                    }
                }
            }

            return final.Distinct().ToList();
        }

        public async Task<Drawing> AddAsync(Drawing document)
        {
            SetAutomaticTags(ref document);
            
            var collection = _firestoreDb.Collection(_collectionNameDrawings);
            var drawingDocument = _converterDrawing.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(drawingDocument);

            return _converterDrawing.ConvertToModel(drawingDocument);
        }


        public async Task AddAsync(Collection document)
        {
            var collection = _firestoreDb.Collection(_collectionNameCollections);
            var drawingDocument = _converterCollection.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(drawingDocument);
        }


        public async Task AddAsync(Inspiration document)
        {
            var collection = _firestoreDb.Collection(_collectionNameInspirations);
            var drawingDocument = _converterInspiration.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(drawingDocument);
        }
    }
}
