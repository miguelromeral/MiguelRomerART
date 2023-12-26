using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Documents
{
    [FirestoreData]
    public class DrawingDocument
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string path { get; set; }

        [FirestoreProperty]
        public string type { get; set; }

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public string title { get; set; }

        [FirestoreProperty]
        public string date { get; set; }

        [FirestoreProperty]
        public string time { get; set; }

        [FirestoreProperty]
        public string product_type { get; set; }

        [FirestoreProperty]
        public string product_name { get; set; }

        [FirestoreProperty]
        public string comment { get; set; }

        [FirestoreProperty]
        public string comment_pros { get; set; }

        [FirestoreProperty]
        public string comment_cons { get; set; }

        [FirestoreProperty]
        public long views { get; set; }
    }
}
