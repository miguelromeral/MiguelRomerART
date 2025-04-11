using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public class DrawingMongoDocument : MongoDocumentBase, IDrawingDocument
{
    [BsonElement("path")]
    public string path { get; set; }

    [BsonElement("path_thumbnail")]
    public string path_thumbnail { get; set; }

    [BsonElement("type")]
    public int type { get; set; }

    [BsonElement("visible")]
    public bool? visible { get; set; }

    [BsonElement("name")]
    public string name { get; set; }

    [BsonElement("model_name")]
    public string model_name { get; set; }

    [BsonElement("title")]
    public string title { get; set; }

    [BsonElement("date")]
    public string date { get; set; }

    [BsonElement("drawingAt")]
    public DateTime drawingAt { get; set; }

    [BsonElement("time")]
    public int? time { get; set; }

    [BsonElement("product_type")]
    public int product_type { get; set; }

    [BsonElement("product_name")]
    public string product_name { get; set; }

    [BsonElement("list_comments")]
    public IEnumerable<string> list_comments { get; set; }

    [BsonElement("list_comments_style")]
    public IEnumerable<string> list_comments_style { get; set; }

    [BsonElement("list_comments_pros")]
    public IEnumerable<string> list_comments_pros { get; set; }

    [BsonElement("list_comments_cons")]
    public IEnumerable<string> list_comments_cons { get; set; }

    [BsonElement("views")]
    public long views { get; set; }

    [BsonElement("filter")]
    public int filter { get; set; }

    [BsonElement("likes")]
    public long likes { get; set; }

    [BsonElement("favorite")]
    public bool favorite { get; set; }

    [BsonElement("reference_url")]
    public string reference_url { get; set; }

    [BsonElement("spotify_url")]
    public string spotify_url { get; set; }

    [BsonElement("twitter_url")]
    public string twitter_url { get; set; }

    [BsonElement("instagram_url")]
    public string instagram_url { get; set; }

    [BsonElement("software")]
    public int software { get; set; }

    [BsonElement("paper")]
    public int paper { get; set; }

    [BsonElement("tags")]
    public IEnumerable<string>? tags { get; set; }

    [BsonElement("score_critic")]
    public int score_critic { get; set; }

    [BsonElement("score_popular")]
    public double score_popular { get; set; }

    [BsonElement("votes_popular")]
    public int votes_popular { get; set; }

    public string GetId() => Id;

    public override async Task<ReplaceOneResult> SetDocumentAsync(IMongoDatabase database, string collection, string documentId)
    {
        var mongoCollection = database.GetCollection<DrawingMongoDocument>(collection);

        var filter = Builders<DrawingMongoDocument>.Filter.Eq(MongoDbDatabase.ID_FIELD, documentId);
        var options = new ReplaceOptions { IsUpsert = true };

        return await mongoCollection.ReplaceOneAsync(filter, this, options);
    }
}
