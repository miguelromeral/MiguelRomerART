using Google.Cloud.Firestore;
using MRA.Infrastructure.Database.Documents.Interfaces;

namespace MRA.Infrastructure.Database.Documents.Firestore;


[FirestoreData]
public class InspirationFirestoreDocument : IInspirationDocument
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Instagram { get; set; }

    [FirestoreProperty]
    public string Twitter { get; set; }

    [FirestoreProperty]
    public string YouTube { get; set; }

    [FirestoreProperty]
    public string Twitch { get; set; }

    [FirestoreProperty]
    public string Pinterest { get; set; }

    [FirestoreProperty]
    public int Type { get; set; }

    public string GetId() => Id;
}