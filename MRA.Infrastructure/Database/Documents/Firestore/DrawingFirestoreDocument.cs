using Google.Cloud.Firestore;
using MRA.Infrastructure.Database.Documents.Interfaces;
using Newtonsoft.Json;

namespace MRA.Infrastructure.Database.Documents.Firestore;

[FirestoreData]
public class DrawingFirestoreDocument : IDrawingDocument
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
    public DateTime drawingAt { get; set; }

    [FirestoreProperty]
    public int? time { get; set; }

    [FirestoreProperty]
    public int product_type { get; set; }

    [FirestoreProperty]
    public string product_name { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> list_comments { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> list_comments_style { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> list_comments_pros { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> list_comments_cons { get; set; }


    [FirestoreProperty]
    public long views { get; set; }

    [FirestoreProperty]
    public int filter { get; set; }

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
    public IEnumerable<string>? tags { get; set; }

    [FirestoreProperty]
    public int score_critic { get; set; }

    [FirestoreProperty]
    public double score_popular { get; set; }

    [FirestoreProperty]
    public int votes_popular { get; set; }

    public string GetId() => Id;
}
