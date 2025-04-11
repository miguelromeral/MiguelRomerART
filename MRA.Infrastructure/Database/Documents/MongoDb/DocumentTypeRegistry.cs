using Google.Protobuf.Reflection;
using MRA.Infrastructure.Configuration;
using MRA.Infrastructure.Database.Documents.Interfaces;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public class DocumentTypeRegistry
{
    private readonly Dictionary<string, Type> _collectionTypeMapping = new Dictionary<string, Type>();

    public DocumentTypeRegistry(AppConfiguration appConfig)
    {
        RegisterDocumentType<InspirationMongoDocument>(appConfig.Database.Collections.Inspirations);
        RegisterDocumentType<DrawingMongoDocument>(appConfig.Database.Collections.Drawings);
        RegisterDocumentType<CollectionMongoDocument>(appConfig.Database.Collections.Collections);
    }

    private void RegisterDocumentType<TDocument>(string collection) where TDocument : IDocument
    {
        _collectionTypeMapping[collection] = typeof(TDocument);
    }

    public Type GetDocumentType(string collection)
    {
        if (_collectionTypeMapping.TryGetValue(collection, out var type))
        {
            return type;
        }

        throw new InvalidOperationException($"No document type registered for collection '{collection}'.");
    }
}
