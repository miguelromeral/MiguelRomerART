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
        public string path_thumbnail { get; set; }

        [FirestoreProperty]
        public int type { get; set; }

        [FirestoreProperty]
        public bool? visible { get; set; }

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public string model_name { get; set; }

        [FirestoreProperty]
        public string title { get; set; }

        [FirestoreProperty]
        public string date { get; set; }

        [FirestoreProperty]
        public int? time { get; set; }

        [FirestoreProperty]
        public int product_type { get; set; }

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

        [FirestoreProperty]
        public long likes { get; set; }

        [FirestoreProperty]
        public bool favorite { get; set; }

        [FirestoreProperty]
        public string reference_url { get; set; }

        [FirestoreProperty]
        public string spotify_url { get; set; }

        [FirestoreProperty]
        public string twitter_url { get; set; }

        [FirestoreProperty]
        public string instagram_url { get; set; }

        [FirestoreProperty]
        public int software { get; set; }

        [FirestoreProperty]
        public int paper { get; set; }

        [FirestoreProperty]
        public List<string>? tags { get; set; }

        [FirestoreProperty]
        public int score_critic { get; set; }
        [FirestoreProperty]
        public double score_popular { get; set; }
        [FirestoreProperty]
        public int votes_popular { get; set; }
    }
}
