using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MongoDB.Driver;
using MRA.Infrastructure.Database.Providers;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public class InspirationMongoDocument : MongoDocumentBase, IInspirationDocument
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("instagram")]
    public string Instagram { get; set; }

    [BsonElement("twitter")]
    public string Twitter { get; set; }

    [BsonElement("youtube")]
    public string YouTube { get; set; }

    [BsonElement("twitch")]
    public string Twitch { get; set; }

    [BsonElement("pinterest")]
    public string Pinterest { get; set; }

    [BsonElement("type")]
    public int Type { get; set; }

    public string GetId() => Id;

    public override async Task<ReplaceOneResult> SetDocumentAsync(IMongoDatabase database, string collection, string documentId)
    {
        var mongoCollection = database.GetCollection<InspirationMongoDocument>(collection);

        var filter = Builders<InspirationMongoDocument>.Filter.Eq(MongoDbDatabase.ID_FIELD, documentId);
        var options = new ReplaceOptions { IsUpsert = true };

        return await mongoCollection.ReplaceOneAsync(filter, this, options);
    }
}