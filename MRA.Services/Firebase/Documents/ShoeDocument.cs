using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Documents
{
    [FirestoreData]
    public class ShoeDocument
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Brand { get; set; }

        [FirestoreProperty]
        public string Price { get; set; }
    }
}
