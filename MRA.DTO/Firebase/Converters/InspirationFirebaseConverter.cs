using MRA.DTO.Firebase.Models;
using MRA.DTO.Models;
using MRA.Infrastructure.Firestore.Documents;

namespace MRA.DTO.Firebase.Converters;

public class InspirationFirebaseConverter : IFirestoreDocumentConverter<InspirationModel, InspirationDocument>
{
    public InspirationModel ConvertToModel(InspirationDocument drawingDocument)
    {
        return new Inspiration
        {
            Id = drawingDocument.Id,
            Name = drawingDocument.name,
            Instagram = drawingDocument.instagram,
            Type = drawingDocument.type,
            Twitter = drawingDocument.twitter,
            YouTube = drawingDocument.youtube,
            Twitch = drawingDocument.twitch,
            Pinterest = drawingDocument.pinterest,
        };
    }

    public InspirationDocument ConvertToDocument(InspirationModel drawing)
    {
        return new InspirationDocument
        {
            Id = drawing.Id,
            name = drawing.Name,
            instagram = drawing.Instagram,
            twitter = drawing.Twitter,
            type = drawing.Type,
            youtube = drawing.YouTube,
            twitch = drawing.Twitch,
            pinterest = drawing.Pinterest,
        };
    }
}
