using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MRA.Infrastructure.Configuration;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Infrastructure.Database.Providers;

public class MongoDbDatabase : IDocumentsDatabase
{
    internal const string ID_FIELD = "_id";

    private readonly AppConfiguration _appConfiguration;
    private readonly DocumentTypeRegistry _typeRegistry;
    private IMongoDatabase _database;

    private IMongoDatabase Database
    {
        get
        {
            if (_database == null)
                InitializeMongoDb();

            return _database;
        }
    }

    public MongoDbDatabase(AppConfiguration appConfig)
    {
        _appConfiguration = appConfig;
        _typeRegistry = new DocumentTypeRegistry(appConfig);
    }

    private void InitializeMongoDb()
    {
        RegisterBsonClass<InspirationMongoDocument>();
        RegisterBsonClass<DrawingMongoDocument>();
        RegisterBsonClass<CollectionMongoDocument>();
        RegisterBsonClass<object>();

        var mongoClient = new MongoClient(_appConfiguration.AzureCosmosDb.ConnectionString);
        _database = mongoClient.GetDatabase(_appConfiguration.AzureCosmosDb.DatabaseName);
    }

    private void RegisterBsonClass<TDocument>()
    {
        BsonClassMap.RegisterClassMap<TDocument>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
        });
    }

    public async Task<IEnumerable<IDocument>> GetAllDocumentsAsync<IDocument>(string collection)
    {
        var mongoCollection = Database.GetCollection<object>(collection);
        var documents = await mongoCollection.Find(FilterDefinition<object>.Empty).ToListAsync();

        var deserializedDocuments = new List<IDocument>();
        foreach (var doc in documents)
        {
            var deserialized = (IDocument)BsonSerializer.Deserialize(doc.ToBson(), GetDocumentType(collection));
            deserializedDocuments.Add(deserialized);
        }

        return deserializedDocuments;
    }

    public async Task<IDocument> GetDocumentAsync<IDocument>(string collection, string documentId)
    {
        var mongoCollection = Database.GetCollection<object>(collection);

        var filter = Builders<object>.Filter.Eq(ID_FIELD, documentId);
        var document = await mongoCollection.Find(filter).FirstOrDefaultAsync();

        if (document == null)
            return default;

        return (IDocument)BsonSerializer.Deserialize(document.ToBson(), GetDocumentType(collection));
    }

    private Type GetDocumentType(string collection) => _typeRegistry.GetDocumentType(collection);


    public async Task<bool> DocumentExistsAsync(string collection, string documentId)
    {
        var mongoCollection = Database.GetCollection<object>(collection);
        var filter = Builders<object>.Filter.Eq(ID_FIELD, documentId);
        return await mongoCollection.Find(filter).AnyAsync();
    }

    public async Task<bool> SetDocumentAsync(string collection, string documentId, IDocument document)
    {
        if(document is not IMongoDocument mongoDocument)
        {
            throw new InvalidOperationException($"Document type mismatch. Can't save to MongoDb");
        }

        var result = await mongoDocument.SetDocumentAsync(Database, collection, documentId);
        return result.ModifiedCount > 0 || result.UpsertedId != null;
    }

    public async Task<bool> DeleteDocumentAsync(string collection, string id)
    {
        var mongoCollection = Database.GetCollection<object>(collection);
        var filter = Builders<object>.Filter.Eq(ID_FIELD, id);
        var result = await mongoCollection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
}