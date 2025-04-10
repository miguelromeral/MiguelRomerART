using Google.Cloud.Firestore;
using MRA.Infrastructure.Database;

namespace MRA.Infrastructure.Firestore.Documents;

[FirestoreData]
public class InspirationDocument : IDocument
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string instagram { get; set; }

    [FirestoreProperty]
    public string twitter { get; set; }

    [FirestoreProperty]
    public string youtube { get; set; }
    [FirestoreProperty]
    public string twitch { get; set; }
    [FirestoreProperty]
    public string pinterest { get; set; }

    [FirestoreProperty]
    public int type { get; set; }

    public string GetId() => Id;
}
