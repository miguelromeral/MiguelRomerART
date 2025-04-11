using Google.Cloud.Firestore;
using MRA.Infrastructure.Database.Documents.Interfaces;
using Newtonsoft.Json;

namespace MRA.Infrastructure.Database.Documents.Firestore;

[FirestoreData]
public class CollectionFirestoreDocument : ICollectionDocument
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string description { get; set; }

    [FirestoreProperty]
    public int order { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> drawingIds { get; set; }

    public string GetId() => Id;
}