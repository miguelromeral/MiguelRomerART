using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Infrastructure.Database.Documents.MongoDb;

public interface IMongoDocument
{
    Task<ReplaceOneResult> SetDocumentAsync(IMongoDatabase database, string collection, string documentId);
}
