using Google.Cloud.Firestore;
using MRA.DTO.Models;
using System.Text.Json.Serialization;

namespace MRA.DTO.Firebase.Models;

public class Collection : CollectionModel
{
    public Collection() : base()
    {
    }

    [JsonIgnore]
    public List<DocumentReference> DrawingsReferences { get; set; }
}
