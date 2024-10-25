using Google.Cloud.Firestore;
using Google.Protobuf.Collections;
using Google.Type;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.Firebase.Interfaces;
using MRA.DTO.Firebase.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;
using MRA.Services.Firebase.Interfaces;
using System.ComponentModel.DataAnnotations;
using MRA.DTO.Firebase.RemoteConfig;
using Google.Cloud.Firestore.V1;
using MRA.DTO.Exceptions;

namespace MRA.Services.Firebase
{
    public class FirestoreService : IFirestoreService
    {
        private readonly FirestoreDb _firestoreDb;
        private string _collectionNameDrawings;
        private string _collectionNameInspirations;
        private string _collectionNameCollections;
        private DrawingFirebaseConverter _converterDrawing;
        private InspirationFirebaseConverter _converterInspiration;
        private CollectionFirebaseConverter _converterCollection;
        private RemoteConfigService? _remoteConfigService;

        public FirestoreService(string projectId, string urlBase)
        {
            _firestoreDb = FirestoreDb.Create(projectId);
            SetConverters(urlBase);
        }

        public FirestoreService(FirestoreDb db, string urlBase)
        {
            _firestoreDb = db;
            SetConverters(urlBase);
        }

        private void SetConverters(string urlBase)
        {
            _converterDrawing = new DrawingFirebaseConverter(urlBase);
            _converterInspiration = new InspirationFirebaseConverter();
            _converterCollection = new CollectionFirebaseConverter();
        }

        public void SetRemoteConfigService(RemoteConfigService service)
        {
            _remoteConfigService = service;
        }

        public void SetCollectionNames(string collectionNameDrawing, string collectionNameCollections, string collectionNameInspirations)
        {
            SetCollectionNameDrawing(collectionNameDrawing);
            SetCollectionNameCollections(collectionNameCollections);
            SetCollectionNameInspirations(collectionNameInspirations);
        }

        public void SetCollectionNameDrawing(string name) => _collectionNameDrawings = name;
        public void SetCollectionNameCollections(string name) => _collectionNameCollections = name;
        public void SetCollectionNameInspirations(string name) => _collectionNameInspirations = name;

        public async Task<List<Drawing>> GetDrawingsAsync()
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            var collection = _firestoreDb.Collection(_collectionNameDrawings);
            var documents = (await collection.OrderByDescending("date").GetSnapshotAsync())
                .Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

            return documents.Select(_converterDrawing.ConvertToModel).ToList();
        }


        public async Task<List<Inspiration>> GetInspirationsAsync()
        {
            if (String.IsNullOrEmpty(_collectionNameInspirations))
            {
                throw new CollectionNameNotProvidedException("Inspirations");
            }

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


        //public async Task<List<Collection>> GetAllCollectionsOrderPositive()
        //{
        //    try
        //    {
        //        Query query = _firestoreDb.Collection(_collectionNameCollections);
        //        query = query.WhereGreaterThanOrEqualTo("order", 0);
        //        var snapshot = (await query.GetSnapshotAsync());
        //        var collections = snapshot.Documents;
        //        var collectionDocs = collections.Select(s => s.ConvertTo<CollectionDocument>()).ToList();
        //        return await HandleAllCollections(collectionDocs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error when getting collections: " + ex.Message);
        //        return new List<Collection>();
        //    }
        //}

        public async Task<List<Collection>> GetCollectionsAsync(List<Drawing> drawings)
        {
            if (String.IsNullOrEmpty(_collectionNameCollections))
            {
                throw new CollectionNameNotProvidedException("Collections");
            }

            try
            {
                var snapshot = (await _firestoreDb.Collection(_collectionNameCollections).GetSnapshotAsync());
                var collections = snapshot.Documents;
                var collectionDocs = collections.Select(s => s.ConvertTo<CollectionDocument>()).ToList();
                return await HandleAllCollections(collectionDocs, drawings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when getting collections: " + ex.Message);
                return new List<Collection>();
            }
        }

        public async Task<Collection> HandleCollection(CollectionDocument collection, List<Drawing> drawings)
        {
            var c = new Collection()
            {
                Id = collection.Id,
                Name = collection.name,
                Order = collection.order,
                Description = collection.description,
                Drawings = new List<Drawing>(),
                DrawingsReferences = collection.drawings
            };

            var documentosReferenciados = new List<DrawingDocument>();
            if (collection.drawings != null)
            {
                foreach (var reference in collection.drawings)
                {
                    var drawing = drawings.Find(x => x.Id == reference.Id);
                    if (drawing != null)
                    {
                        c.Drawings.Add(drawing);
                    }
                }
            }

            return c;
        }

        public async Task<List<DocumentReference>> SetDrawingsReferencesAsync(string[] ids)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            var list = new List<DocumentReference>();

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var tmp = _firestoreDb.Document(_collectionNameDrawings + "/" + id);
                    list.Add(tmp);
                }
            }

            return list;
        }

        public async Task<List<Collection>> HandleAllCollections(List<CollectionDocument> collectionDocs, List<Drawing> drawings)
        {
            try
            {
                var listCollections = new List<Collection>();

                foreach(var collection in collectionDocs)
                {
                    listCollections.Add(await HandleCollection(collection, drawings));
                }
                return listCollections;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when getting collections: " + ex.Message);
                return new List<Collection>();
            }
        }

        public async Task<FilterResults> FilterGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections)
        {
            if (filter.OnlyVisible)
            {
                drawings = drawings.Where(x => x.Visible).ToList();
            }
            if (filter.Favorites)
            {
                drawings = drawings.Where(x => x.Favorite).ToList();
            }
            if (!String.IsNullOrEmpty(filter.TextQuery))
            {
                var tags = DeleteAndAdjustTags(filter.Tags).Select(x => x.ToLower());
                drawings = drawings.Where(x =>
                    x.Tags.Join(tags, t1 => t1.ToLower(), t2 => t2, (t1, t2) => t1.Contains(t2)).Any()).ToList();
            }
            if (!String.IsNullOrEmpty(filter.ProductName))
            {
                if (filter.ProductName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.ProductName.Equals("")).ToList();
                }
                else
                {
                drawings = drawings.Where(x =>
                    x.ProductName.Contains(filter.ProductName)).ToList();

                }
            }
            if (!String.IsNullOrEmpty(filter.CharacterName))
            {
                if (filter.CharacterName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.Name.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.Name.Contains(filter.CharacterName)).ToList();

                }
            }
            if (!String.IsNullOrEmpty(filter.ModelName))
            {
                if (filter.ModelName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.ModelName.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.ModelName.Contains(filter.ModelName)).ToList();

                }
            }
            if(filter.Type > -1)
            {
                drawings = drawings.Where(x => x.Type == filter.Type).ToList();
            }
            if (filter.ProductType > -1)
            {
                drawings = drawings.Where(x => x.ProductType == filter.ProductType).ToList();
            }
            if (filter.Software > 0)
            {
                drawings = drawings.Where(x => x.Software == filter.Software).ToList();
            }
            if (filter.Paper > 0)
            {
                drawings = drawings.Where(x => x.Paper == filter.Paper).ToList();
            }
            if(filter.Spotify != null)
            {

                if (filter.Spotify ?? false)
                {
                    drawings = drawings.Where(x => !x.SpotifyUrl.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x => x.SpotifyUrl.Equals("")).ToList();
                }
            }
            if (!String.IsNullOrEmpty(filter.Collection))
            {
                var collection = collections.Find(x => x.Id.Equals(filter.Collection));

                if (collection != null)
                {
                    var idsCollection = collection.Drawings.Select(x => x.Id).ToList();
                    drawings = drawings.Where(d => idsCollection.Contains(d.Id)).ToList();
                }
            }


            if (filter.OnlyFilterCollection())
            {
                var selectedCollection = collections.Find(x => x.Id == filter.Collection);
                if (selectedCollection != null) {
                    var list = new List<Drawing>();
                    foreach (var id in selectedCollection.Drawings.Select(x => x.Id)) {
                        var d = drawings.Find(x => x.Id == id);
                        if (d != null)
                        {
                            list.Add(d);
                        }
                    }
                    drawings = list;
                }
            }
            else
            {

                switch (filter.Sortby)
                {
                    case "date-asc":
                        drawings = drawings.OrderBy(x => x.Date).ToList();
                        break;
                    case "date-desc":
                        drawings = drawings.OrderByDescending(x => x.Date).ToList();
                        break;
                    case "name-asc":
                        drawings = drawings.OrderBy(x => x.Name).ToList();
                        break;
                    case "name-desc":
                        drawings = drawings.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "kudos-asc":
                        drawings = drawings.OrderBy(x => x.Likes).ToList();
                        break;
                    case "kudos-desc":
                        drawings = drawings.OrderByDescending(x => x.Likes).ToList();
                        break;
                    case "views-asc":
                        drawings = drawings.OrderBy(x => x.Views).ToList();
                        break;
                    case "views-desc":
                        drawings = drawings.OrderByDescending(x => x.Views).ToList();
                        break;
                    case "scorem-asc":
                        drawings = drawings.OrderBy(x => x.ScoreCritic).ThenBy(x => x.ScorePopular).ThenBy(x => x.VotesPopular).ToList();
                        break;
                    case "scorem-desc":
                        drawings = drawings.OrderByDescending(x => x.ScoreCritic).ThenByDescending(x => x.ScorePopular).ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "scoreu-asc":
                        drawings = drawings.Where(x => x.VotesPopular > 0).OrderBy(x => x.ScorePopular).ThenBy(x => x.VotesPopular).ToList();
                        break;
                    case "scoreu-desc":
                        drawings = drawings.Where(x => x.VotesPopular > 0).OrderByDescending(x => x.ScorePopular).ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "time-asc":
                        drawings = drawings.OrderBy(x => x.Time).ToList();
                        break;
                    case "time-desc":
                        drawings = drawings.OrderByDescending(x => x.Time).ToList();
                        break;
                    default:

                        double wDate = 0, wCritic = 0, wPopular = 0, wFavorite = 0;
                        int wMonths = 0;

                        if (_remoteConfigService != null)
                        {
                            wDate = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityDateWeight);
                            wMonths = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityDateMonths);
                            wCritic = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityCriticWeight);
                            wPopular = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityPopularWeight);
                            wFavorite = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityFavoriteWeight);
                        }

                        foreach (var d in drawings)
                        {
                            d.CalculatePopularity(wDate, wMonths, wCritic, wPopular, wFavorite);
                        }

                        drawings = drawings.OrderByDescending(x => x.Popularity).ToList();
                        break;
                }
            }

            var results = new FilterResults(drawings);
            var ids = drawings.Select(x => x.Id).ToList();
            results.FilteredCollections = collections
                .Where(c => c.Drawings.Any(d => ids.Contains(d.Id)))
                .Select(x => x.Id)
                .ToList();


            if (filter.PageSize > 0 && filter.PageNumber > 0)
            {
                // Saltar los elementos de las páginas anteriores
                drawings = drawings.Skip((filter.PageNumber - 1) * filter.PageSize)
                    // Tomar solo los elementos de la página actual
                    .Take(filter.PageSize)  
                    .ToList();
            }



            //foreach (var d in drawings)
            //{
            //    Debug.WriteLine($"{d.Id.PadRight(30)} ({d.Name.PadRight(30)}): [{d.Popularity.ToString("#.###").PadRight(5)} == " +
            //        $"{d.PopularityDate.ToString("#.##").PadRight(5)} + {d.PopularityCritic.ToString("#.##").PadRight(5)} + {d.PopularityPopular.ToString("#.##").PadRight(5)}" +
            //        $" + {d.PopularityFavorite.ToString("#.##").PadRight(5)} ]");
            //}


            results.UpdatefilteredDrawings(drawings);
            return results;
        }

        public async Task<List<Drawing>> Filter(DrawingFilter filter)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            try
            {
                Query query = _firestoreDb.Collection(_collectionNameDrawings);

                //query = query.WhereNotEqualTo("visible", false);

                //if (filter.OnlyVisible)
                //{
                //    query = query.WhereEqualTo("visible", true);
                //}
                if (filter.Favorites)
                {
                    query = query.WhereEqualTo("favorite", true);
                }
                if (!String.IsNullOrEmpty(filter.TextQuery))
                {
                    query = query.WhereArrayContainsAny("tags", DeleteAndAdjustTags(filter.Tags));
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
                if (filter.Spotify != null)
                {
                    if (filter.Spotify ?? false)
                    {
                        query = query.WhereNotEqualTo("spotify_url", "");
                    }
                    else
                    {
                        query = query.WhereEqualTo("spotify_url", "");
                    }
                }

                //// Aplicar la ordenación
                //switch (filter.Sortby)
                //{
                //    case "date-asc":
                //        query = query.OrderBy("date");
                //        break;
                //    case "date-desc":
                //        query = query.OrderByDescending("date");
                //        break;
                //    case "name-asc":
                //        query = query.OrderBy("name");
                //        break;
                //    case "name-desc":
                //        query = query.OrderByDescending("name");
                //        break;
                //    case "kudos-asc":
                //        query = query.OrderBy("likes");
                //        break;
                //    case "kudos-desc":
                //        query = query.OrderByDescending("likes");
                //        break;
                //    case "views-asc":
                //        query = query.OrderBy("views");
                //        break;
                //    case "views-desc":
                //        query = query.OrderByDescending("views");
                //        break;
                //    case "scorem-asc":
                //        query = query.OrderBy("score_critic");
                //        break;
                //    case "scorem-desc":
                //        query = query.OrderByDescending("score_critic");
                //        break;
                //    case "scoreu-asc":
                //        query = query.OrderBy("score_popular").OrderByDescending("votes_popular");
                //        break;
                //    case "scoreu-desc":
                //        query = query.OrderByDescending("score_popular").OrderByDescending("votes_popular");
                //        break;
                //    case "time-asc":
                //        query = query.OrderBy("time");
                //        break;
                //    case "time-desc":
                //        query = query.OrderByDescending("time");
                //        break;
                //    default:
                //        query = query.OrderByDescending("date");
                //        break;
                //}

                if (filter.PageSize > 0 && filter.PageNumber > 0)
                {
                    query = query.Limit(filter.PageSize);

                    // Obtener el documento de inicio basado en la página anterior
                    var previousDocumentsQuery = query.Limit((filter.PageNumber- 1) * filter.PageSize);
                    var previousDocuments = await previousDocumentsQuery.GetSnapshotAsync();
                    var lastDocumentFromPreviousPage = previousDocuments.Documents.LastOrDefault();
                    if (lastDocumentFromPreviousPage != null)
                    {
                        query = query.StartAfter(lastDocumentFromPreviousPage);
                    }
                }

                var documents = (await query.GetSnapshotAsync()).Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

                var list = documents.Select(_converterDrawing.ConvertToModel).ToList();


                if (!String.IsNullOrEmpty(filter.Collection))
                {
                    var collection = (await GetCollectionsAsync(list)).Find(x => x.Id.Equals(filter.Collection));

                    if(collection != null)
                    {
                        var idsCollection = collection.Drawings.Select(x => x.Id).ToList();

                        list = list.Where(d => idsCollection.Contains(d.Id)).ToList();
                    }
                }

                if (filter.OnlyVisible)
                {
                    list = list.Where(x => x.Visible).ToList();
                }

                return list;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error when filtering documents: " + ex.Message);
            }
            return new List<Drawing>();
        }

        public async Task<Drawing> FindDrawingByIdAsync(string documentId, bool updateViews = false)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            try
            {
                DocumentReference docRef = _firestoreDb.Collection(_collectionNameDrawings).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    if (updateViews)
                    {
                        await UpdateViewsAsync(documentId);
                    }

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
            if (String.IsNullOrEmpty(_collectionNameInspirations))
            {
                throw new CollectionNameNotProvidedException("Inspirations");
            }

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

        public async Task<Collection> FindCollectionByIdAsync(string documentId, List<Drawing> drawings)
        {
            if (String.IsNullOrEmpty(_collectionNameCollections))
            {
                throw new CollectionNameNotProvidedException("Collections");
            }

            try
            {
                DocumentReference docRef = _firestoreDb.Collection(_collectionNameCollections).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var converter = new CollectionFirebaseConverter();
                    var tmp = snapshot.ConvertTo<CollectionDocument>();
                    return await HandleCollection(tmp, drawings);
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task RemoveCollectionAsync(string id)
        {
            if (String.IsNullOrEmpty(_collectionNameCollections))
            {
                throw new CollectionNameNotProvidedException("Collections");
            }

            DocumentReference docRef = _firestoreDb.Collection(_collectionNameCollections).Document(id);
            await docRef.DeleteAsync();
        }


        public async Task UpdateViewsAsync(string documentId)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

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

        public async Task UpdateLikesAsync(string documentId)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

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


        public async Task<VoteSubmittedModel> VoteAsync(string documentId, int score)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            // Realiza la transacción para actualizar la propiedad "views"
            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                var model = new VoteSubmittedModel();
                try
                {
                    if (score > 100) score = 100;
                    if (score < 0) score = 0;

                    DocumentReference docRef = _firestoreDb.Collection(_collectionNameDrawings).Document(documentId);

                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);


                    if (snapshot.ContainsField("score_popular") && snapshot.ContainsField("votes_popular"))
                    {
                        int nVotes = snapshot.GetValue<int>("votes_popular");
                        double average = snapshot.GetValue<double>("score_popular");

                        model.NewVotes = nVotes + 1;
                        model.NewScore = ((average * nVotes) + score) / (nVotes + 1);

                        transaction.Update(docRef, "score_popular", model.NewScore);
                        transaction.Update(docRef, "votes_popular", model.NewVotes);
                    }
                    else
                    {

                        model.NewVotes = 1;
                        model.NewScore = score;

                        // Si no existe, crea la propiedad "views" con el valor inicial de 1
                        transaction.Set(docRef, new { votes_popular = model.NewVotes, score_popular = model.NewScore }, SetOptions.MergeAll);
                    }

                }
                catch (Exception ex)
                {
                    model.NewVotes = -1;
                    Debug.WriteLine("Error when updating score for document '" + documentId + "': " + ex.Message);
                }
                return model;
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
                .Replace("ä", "a")
                .Replace("ë", "e")
                .Replace("ï", "i")
                .Replace("ö", "o")
                .Replace("ü", "u")
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

        public async Task<Drawing> AddDrawingAsync(Drawing document)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            SetAutomaticTags(ref document);
            
            var collection = _firestoreDb.Collection(_collectionNameDrawings);
            var drawingDocument = _converterDrawing.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(drawingDocument);

            return _converterDrawing.ConvertToModel(drawingDocument);
        }


        public async Task<Collection> AddCollectionAsync(Collection document, List<Drawing> drawings)
        {
            if (String.IsNullOrEmpty(_collectionNameCollections))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            var collection = _firestoreDb.Collection(_collectionNameCollections);
            var collectionDocument = _converterCollection.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(collectionDocument);

            return await HandleCollection(collectionDocument, drawings);
        }

        public DocumentReference GetDbDocument(string path)
        {
            return _firestoreDb.Document(path);
        }

        public DocumentReference GetDbDocumentDrawing(string id)
        {
            if (String.IsNullOrEmpty(_collectionNameDrawings))
            {
                throw new CollectionNameNotProvidedException("Drawings");
            }

            return _firestoreDb.Document(_collectionNameDrawings + "/" + id);
        }


        public async Task AddAsync(Inspiration document)
        {
            if (String.IsNullOrEmpty(_collectionNameInspirations))
            {
                throw new CollectionNameNotProvidedException("Inspirations");
            }

            var collection = _firestoreDb.Collection(_collectionNameInspirations);
            var drawingDocument = _converterInspiration.ConvertToDocument(document);

            // Obtiene una referencia al documento específico en la colección
            DocumentReference docRef = collection.Document(document.Id);

            // Inserta o actualiza el documento con los datos especificados
            await docRef.SetAsync(drawingDocument);
        }
    }
}
