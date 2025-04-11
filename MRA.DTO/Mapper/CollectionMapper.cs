using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Models;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;

namespace MRA.DTO.Mapper;

public class CollectionMapper : IDocumentMapper<CollectionModel, ICollectionDocument>
{
    public CollectionModel ConvertToModel(ICollectionDocument collectionDocument)
    {
        return new CollectionModel
        {
            Id = collectionDocument.Id,
            Name = collectionDocument.name,
            Description = collectionDocument.description,
            Order = collectionDocument.order,
            DrawingIds = collectionDocument.drawingIds
        };
    }

    public ICollectionDocument ConvertToDocument(CollectionModel collection)
    {
        return new CollectionMongoDocument
        {
            Id = collection.Id,
            name = collection.Name,
            description = collection.Description,
            order = collection.Order,
            drawingIds = collection.DrawingIds.ToList()
        };
    }
}
