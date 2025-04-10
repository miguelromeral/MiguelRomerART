using Google.Cloud.Firestore;
using MRA.Infrastructure.Database;

namespace MRA.Infrastructure.Firestore.Documents;

[FirestoreData]
public class CollectionDocument : IDocument
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
    public List<DocumentReference> drawings { get; set; }

    [FirestoreProperty]
    public List<string> drawingIds { get; set; }

    public string GetId() => Id;
}