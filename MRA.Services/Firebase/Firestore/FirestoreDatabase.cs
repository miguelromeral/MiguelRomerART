using Google.Cloud.Firestore;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Xml.Linq;

namespace MRA.Services.Firebase.Firestore
{
    public class FirestoreDatabase : IFirestoreDatabase
    {
        private FirestoreDb _firestoreDb;

        public FirestoreDatabase()
        {
        }

        public void Create(string projectId, Google.Cloud.Firestore.V1.FirestoreClient client = null)
        {
            _firestoreDb = FirestoreDb.Create(projectId, client);
        }

        //public CollectionReference Collection(string path) => _firestoreDb.Collection(path);

        public async Task<List<T>> GetAllDocumentsAsync<T>(string collection) =>
            (await _firestoreDb.Collection(collection).GetSnapshotAsync())
                .Documents.Select(s => s.ConvertTo<T>()).ToList();

        public DocumentReference GetDocumentReference(string collection, string documentId) => _firestoreDb.Collection(collection).Document(documentId);

        public async Task<bool> DocumentExistsAsync(string collection, string documentId)
        {
            DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists;
        }
        public async Task<T> GetDocumentAsync<T>(string collection, string documentId)
        {
            DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.ConvertTo<T>();
        }

        //public async Task<T> RunTransactionAsync<T>(Func<Google.Cloud.Firestore.Transaction, Task<T>> callback, Google.Cloud.Firestore.TransactionOptions options = null, CancellationToken cancellationToken = default)
        //{
        //    return await _firestoreDb.RunTransactionAsync(callback, options, cancellationToken);
        //}

        //public async Task RunTransactionAsync(Func<Google.Cloud.Firestore.Transaction, Task> callback, Google.Cloud.Firestore.TransactionOptions options = null, CancellationToken cancellationToken = default)
        //{
        //    await _firestoreDb.RunTransactionAsync(callback, options, cancellationToken);
        //}

        public async Task<Google.Cloud.Firestore.WriteResult> SetDocumentAsync<T>(string collection, string documentId, T document)
        {
            DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);
            return await docRef.SetAsync(document);
        }

        public async Task<bool> UpdateViewsAsync(string collection, string documentId)
        {
            string fieldNameViews = "views";
            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);
                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);

                    if (snapshot.ContainsField(fieldNameViews))
                    {
                        // Si existe, obtiene el valor actual de "views" y le suma uno
                        long currentViews = snapshot.GetValue<long>(fieldNameViews);
                        long newViews = currentViews + 1;

                        // Actualiza la propiedad "views" en el documento
                        transaction.Update(docRef, fieldNameViews, newViews);
                    }
                    else
                    {
                        // Si no existe, crea la propiedad "views" con el valor inicial de 1
                        transaction.Set(docRef, new { views = 1 }, SetOptions.MergeAll);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error when updating views for document '" + documentId + "': " + ex.Message);
                }
                return false;
            });
        }

        public async Task<bool> UpdateLikesAsync(string collection, string documentId)
        {
            string fieldNameLikes = "likes";
            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);
                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);


                    if (snapshot.ContainsField(fieldNameLikes))
                    {
                        long currentViews = snapshot.GetValue<long>(fieldNameLikes);
                        long newViews = currentViews + 1;

                        transaction.Update(docRef, fieldNameLikes, newViews);
                    }
                    else
                    {
                        // Si no existe, crea la propiedad "likes" con el valor inicial de 1
                        transaction.Set(docRef, new { likes = 1 }, SetOptions.MergeAll);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error when updating likes for document '" + documentId + "': " + ex.Message);
                }
                return false;
            });
        }


        public async Task<VoteSubmittedModel> VoteAsync(string collection, string documentId, int score)
        {
            string fieldNameVotesPopular = "votes_popular";
            string fieldNameScorePopular = "score_popular";
            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                var model = new VoteSubmittedModel();
                try
                {
                    if (score > 100) score = 100;
                    if (score < 0) score = 0;

                    DocumentReference docRef = _firestoreDb.Collection(collection).Document(documentId);

                    // Obtiene el documento actual
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);


                    if (snapshot.ContainsField(fieldNameScorePopular) && snapshot.ContainsField(fieldNameVotesPopular))
                    {
                        int nVotes = snapshot.GetValue<int>(fieldNameVotesPopular);
                        double average = snapshot.GetValue<double>(fieldNameScorePopular);

                        model.NewVotes = nVotes + 1;
                        model.NewScore = ((average * nVotes) + score) / (nVotes + 1);

                        transaction.Update(docRef, fieldNameScorePopular, model.NewScore);
                        transaction.Update(docRef, fieldNameVotesPopular, model.NewVotes);
                    }
                    else
                    {
                        model.NewVotes = 1;
                        model.NewScore = score;

                        transaction.Set(docRef, new { votes_popular = model.NewVotes, score_popular = model.NewScore }, SetOptions.MergeAll);
                    }
                    model.Success = true;
                }
                catch (Exception ex)
                {
                    model.NewVotes = -1;
                    Debug.WriteLine("Error when updating score for document '" + documentId + "': " + ex.Message);
                    model.Success = false;
                }
                return model;
            });
        }

        public async Task<WriteResult> DeleteDocumentAsync(string collection, string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(collection).Document(id);
            return await docRef.DeleteAsync();
        }
    }
}
