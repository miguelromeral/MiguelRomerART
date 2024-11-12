using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Firestore
{
    public interface IFirestoreDatabase
    {
        CollectionReference Collection(string path);
        DocumentReference Document(string path);
        Task<T> RunTransactionAsync<T>(Func<Transaction, Task> callback, TransactionOptions options = null, CancellationToken cancellationToken = default);
    }
}
