using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Documents
{
    [FirestoreData]
    public class InspirationDocument
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public string instagram { get; set; }

        [FirestoreProperty]
        public string twitter { get; set; }

        [FirestoreProperty]
        public int type{ get; set; }
    }
}
