using Google.Cloud.Firestore;
using Google.Protobuf.Compiler;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Converters
{
    internal class CollectionFirebaseConverter : IFirebaseConverter<Collection, CollectionDocument>
    {
        public Collection ConvertToModel(CollectionDocument collectionDocument)
        {
            return new Collection
            {
                Id = collectionDocument.Id,
                Name = collectionDocument.name,
                Description = collectionDocument.description,
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
                drawings = collection.DrawingsReferences
            };
        }
    }
}
