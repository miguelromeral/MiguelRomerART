using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using MRA.Services.Firebase.Converters;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
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
            _converter = new DrawingFirebaseConverter();
        }

        public async Task<List<Drawing>> GetAll()
        {
            var collection = _firestoreDb.Collection(_collectionName);
            var snapshot = await collection.GetSnapshotAsync();

            var shoeDocuments = snapshot.Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

            return shoeDocuments.Select(_converter.ConvertToModel).ToList();
        }

        public async Task AddAsync(Drawing document)
        {
            var collection = _firestoreDb.Collection(_collectionName);
            var drawingDocument = _converter.ConvertToDocument(document);

            await collection.AddAsync(drawingDocument);
        }
    }
}
