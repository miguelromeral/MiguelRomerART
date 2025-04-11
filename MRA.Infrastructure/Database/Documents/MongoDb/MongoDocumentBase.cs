using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public abstract class MongoDocumentBase : IMongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string _id { get; set; }

    [BsonIgnore]
    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public abstract Task<ReplaceOneResult> SetDocumentAsync(IMongoDatabase database, string collection, string documentId);
}
