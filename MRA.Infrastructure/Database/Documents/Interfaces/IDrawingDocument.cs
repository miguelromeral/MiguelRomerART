namespace MRA.Infrastructure.Database.Documents.Interfaces;

public interface IDrawingDocument : IDocument
{
    string Id { get; set; }
    string path { get; set; }
    string path_thumbnail { get; set; }
    int type { get; set; }
    bool? visible { get; set; }
    string name { get; set; }
    string model_name { get; set; }
    string title { get; set; }
    string date { get; set; }
    DateTime drawingAt { get; set; }
    int? time { get; set; }
    int product_type { get; set; }
    string product_name { get; set; }
    IEnumerable<string> list_comments { get; set; }
    IEnumerable<string> list_comments_style { get; set; }
    IEnumerable<string> list_comments_pros { get; set; }
    IEnumerable<string> list_comments_cons { get; set; }
    long views { get; set; }
    int filter { get; set; }
    long likes { get; set; }
    bool favorite { get; set; }
    string reference_url { get; set; }
    string spotify_url { get; set; }
    string twitter_url { get; set; }
    string instagram_url { get; set; }
    int software { get; set; }
    int paper { get; set; }
    IEnumerable<string>? tags { get; set; }
    int score_critic { get; set; }
    double score_popular { get; set; }
    int votes_popular { get; set; }
}
