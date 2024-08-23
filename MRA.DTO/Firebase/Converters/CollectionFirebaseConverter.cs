using Google.Cloud.Firestore;
using Google.Protobuf.Compiler;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Interfaces;
using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Converters
{
    public class CollectionFirebaseConverter : IFirebaseConverter<Collection, CollectionDocument>
    {
        public Collection ConvertToModel(CollectionDocument collectionDocument)
        {
            return new Collection
            {
                Id = collectionDocument.Id,
                Name = collectionDocument.name,
                Description = collectionDocument.description,
                Order = collectionDocument.order,
                DrawingsReferences = collectionDocument.drawings
            };
        }

        public CollectionDocument ConvertToDocument(Collection collection)
        {
            return new CollectionDocument
            {
                Id = collection.Id,
                name = collection.Name,
                description = collection.Description,
                order = collection.Order,
                drawings = collection.DrawingsReferences
            };
        }
    }
}
