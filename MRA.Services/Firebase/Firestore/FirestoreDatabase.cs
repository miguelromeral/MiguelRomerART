using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Firestore
{
    public class FirestoreDatabase : IFirestoreDatabase
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreDatabase(FirestoreDb db)
        {
            _firestoreDb = db;
        }

        public CollectionReference Collection(string path) => _firestoreDb.Collection(path);
        public DocumentReference Document(string path) => _firestoreDb.Document(path);

        public async Task RunTransactionAsync<T>(Func<Transaction, Task<T>> callback, TransactionOptions options = null, CancellationToken cancellationToken = default) => await _firestoreDb.RunTransactionAsync(callback, options, cancellationToken);

        public Task<T> RunTransactionAsync<T>(Func<Transaction, Task> callback, TransactionOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
