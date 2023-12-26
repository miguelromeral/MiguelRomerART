using Google.Cloud.Firestore;
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
        private readonly string _collectionName;
        private readonly DrawingFirebaseConverter _converter;

        public FirestoreService(IConfiguration configuration, FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _collectionName = configuration.GetValue<string>("Firebase:Collection");
            _urlBase = configuration.GetValue<string>("AzureStorage:BlobPath");
            _converter = new DrawingFirebaseConverter(_urlBase);
        }

        public async Task<List<Drawing>> GetAll()
        {
            var collection = _firestoreDb.Collection(_collectionName);
            //var snapshot = await collection.OrderByDescending("date").OrderBy(FieldPath.DocumentId).GetSnapshotAsync();
            
            var documents = (await collection/*.WhereEqualTo("type", "traditional")*/.OrderByDescending("date").GetSnapshotAsync())
                .Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

            //var documentsNoDate = (await collection.Where(new  WhereLessThan("date", DateTime.MinValue).GetSnapshotAsync())
            //    .Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

            //documents.AddRange(documentsNoDate);

            return documents.Select(_converter.ConvertToModel).ToList();
        }

        public async Task<List<Drawing>> Filter(FilterDrawingModel filter)
        {
            try
            {
                Query query = _firestoreDb.Collection(_collectionName);

                if (!String.IsNullOrEmpty(filter.Type) && !filter.Type.Equals("all"))
                {
                    query = query.WhereEqualTo("type", filter.Type);
                }
                if (!String.IsNullOrEmpty(filter.Textquery))
                {
                    query = query.WhereArrayContains("name", filter.Textquery);
                }

                var documents = (await query.GetSnapshotAsync()).Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

                return documents.Select(_converter.ConvertToModel).ToList();
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
                DocumentReference docRef = _firestoreDb.Collection(_collectionName).Document(documentId);

                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    await UpdateViews(documentId);
                    return _converter.ConvertToModel(snapshot.ConvertTo<DrawingDocument>());
                }
                 
                return null;
            }catch(Exception ex)
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
                    DocumentReference docRef = _firestoreDb.Collection(_collectionName).Document(documentId);

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

        public async Task AddAsync(Drawing document)
        {
            var collection = _firestoreDb.Collection(_collectionName);
            var drawingDocument = _converter.ConvertToDocument(document);

            await collection.AddAsync(drawingDocument);
        }
    }
}
