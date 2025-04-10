using MRA.DTO.Firebase.Converters.Interfaces;
using MRA.DTO.Models;
using MRA.Infrastructure.Firestore.Documents;

namespace MRA.DTO.Firebase.Converters;

public class CollectionFirebaseConverter : IFirestoreDocumentConverter<CollectionModel, CollectionDocument>
{
    public CollectionModel ConvertToModel(CollectionDocument collectionDocument)
    {
        return new CollectionModel
        {
            Id = collectionDocument.Id,
            Name = collectionDocument.name,
            Description = collectionDocument.description,
            Order = collectionDocument.order,
            DrawingIds = collectionDocument.drawingIds
            //DrawingsReferences = collectionDocument.drawings
        };
    }

    public CollectionDocument ConvertToDocument(CollectionModel collection)
    {
        return new CollectionDocument
        {
            Id = collection.Id,
            name = collection.Name,
            description = collection.Description,
            order = collection.Order,
            drawingIds = collection.DrawingIds.ToList()
            //drawings = collection.DrawingsReferences
        };
    }
}
