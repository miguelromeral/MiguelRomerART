using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Infrastructure.Database.Providers;

public class MongoDbDatabase : IDocumentsDatabase
{
    internal const string ID_FIELD = "_id";

    private readonly AppSettings _appConfiguration;
    private readonly DocumentTypeRegistry _typeRegistry;
    private readonly ILogger<MongoDbDatabase> _logger;

    private readonly object _initLock = new object();
    private static IMongoDatabase? _database = null;

    private IMongoDatabase Database
    {
        get
        {
            if (_database == null)
            {
                lock (_initLock)
                {
                    if (_database == null)
                    {
                        InitializeMongoDb();
                    }
                }
            }

            return _database;
        }
    }

    public MongoDbDatabase(AppSettings appConfig, ILogger<MongoDbDatabase> logger)
    {
        _appConfiguration = appConfig;
        _logger = logger;
        _typeRegistry = new DocumentTypeRegistry(appConfig);
    }

    private void InitializeMongoDb()
    {
        _logger.LogInformation("Initializing MongoDb Database. Registering document classes.");
        RegisterBsonClass(typeof(InspirationMongoDocument));
        RegisterBsonClass(typeof(DrawingMongoDocument));
        RegisterBsonClass(typeof(CollectionMongoDocument));

        var mongoClient = new MongoClient(_appConfiguration.AzureCosmosDb.ConnectionString);
        _database = mongoClient.GetDatabase(_appConfiguration.AzureCosmosDb.DatabaseName);
    }

    private void RegisterBsonClass(Type documentType)
    {
        if (!BsonClassMap.IsClassMapRegistered(documentType))
        {
            var classMap = new BsonClassMap(documentType);
            classMap.AutoMap();
            classMap.SetIgnoreExtraElements(true);
            BsonClassMap.RegisterClassMap(classMap);
        }
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