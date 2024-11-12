using Google.Cloud.Firestore;
using MRA.DTO.Firebase.Models;

namespace MRA.Services.Firebase.Firestore
{
    public interface IFirestoreDatabase
    {
        void Create(string projectId, Google.Cloud.Firestore.V1.FirestoreClient client = null);
        //CollectionReference Collection(string path);
        Task<List<T>> GetAllDocumentsAsync<T>(string collection);
        Task<bool> DocumentExistsAsync(string collection, string documentId);
        DocumentReference GetDocumentReference(string collection, string documentId);
        Task<T> GetDocumentAsync<T>(string collection, string documentId);
        //Task<T> RunTransactionAsync<T>(Func<Transaction, Task<T>> callback, Google.Cloud.Firestore.TransactionOptions options = null, CancellationToken cancellationToken = default);
        //Task RunTransactionAsync(Func<Transaction, Task> callback, Google.Cloud.Firestore.TransactionOptions options = null, CancellationToken cancellationToken = default);
        Task<WriteResult> SetDocumentAsync<T>(string collection, string documentId, T document);
        Task<bool> UpdateViewsAsync(string collection, string documentId);
        Task<bool> UpdateLikesAsync(string collection, string documentId);
        Task<VoteSubmittedModel> VoteAsync(string collection, string documentId, int score);
        Task<WriteResult> DeleteDocumentAsync(string collection, string id);

    }
}
