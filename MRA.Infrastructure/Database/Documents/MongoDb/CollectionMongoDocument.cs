using Google.Cloud.Firestore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MongoDB.Driver;
using MRA.Infrastructure.Database.Providers;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public class CollectionMongoDocument : MongoDocumentBase, ICollectionDocument
{
    [BsonElement("name")]
    public string name { get; set; }

    [BsonElement("description")]
    public string description { get; set; }

    [BsonElement("order")]
    public int order { get; set; }

    [BsonElement("drawingIds")]
    public IEnumerable<string> drawingIds { get; set; }

    public string GetId() => Id;

    public override async Task<ReplaceOneResult> SetDocumentAsync(IMongoDatabase database, string collection, string documentId)
    {
        var mongoCollection = database.GetCollection<CollectionMongoDocument>(collection);

        var filter = Builders<CollectionMongoDocument>.Filter.Eq(MongoDbDatabase.ID_FIELD, documentId);
        var options = new ReplaceOptions { IsUpsert = true };

        return await mongoCollection.ReplaceOneAsync(filter, this, options);
    }
}
