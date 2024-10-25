using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Exceptions
{
    public class CollectionNameNotProvidedException : Exception
    {
        public CollectionNameNotProvidedException()
        {
        }

        public CollectionNameNotProvidedException(string collectionName)
            : base(CustomMessage(collectionName))
        {
        }

        public CollectionNameNotProvidedException(string collectionName, Exception inner)
            : base(CustomMessage(collectionName), inner)
        {
        }

        public static string CustomMessage(string name) => $"The collection with name \"{name}\" was not provided.";
    }
}
