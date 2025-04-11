using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace MRA.Infrastructure.Database.Documents.Interfaces;

public interface ICollectionDocument : IDocument
{
    string Id { get; set; }
    string name { get; set; }
    string description { get; set; }
    int order { get; set; }
    IEnumerable<string> drawingIds { get; set; }
}
