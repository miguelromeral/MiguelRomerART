using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Documents
{
    [FirestoreData]
    public class CollectionDocument
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public string description { get; set; }

        [FirestoreProperty]
        public int order { get; set; }

        [FirestoreProperty]
        public List<DocumentReference> drawings { get; set; }
    }
}
