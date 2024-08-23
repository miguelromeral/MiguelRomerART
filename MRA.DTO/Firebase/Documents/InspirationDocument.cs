using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Documents
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
        public string youtube { get; set; }
        [FirestoreProperty]
        public string twitch { get; set; }
        [FirestoreProperty]
        public string pinterest { get; set; }

        [FirestoreProperty]
        public int type{ get; set; }
    }
}
