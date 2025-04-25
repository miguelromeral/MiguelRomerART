using MRA.DTO.Enums.Inspirations;
using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Models;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;

namespace MRA.DTO.Mapper;

public class InspirationMapper : IDocumentMapper<InspirationModel, IInspirationDocument>
{
    public InspirationModel ConvertToModel(IInspirationDocument drawingDocument)
    {
        return new InspirationModel
        {
            Id = drawingDocument.Id,
            Name = drawingDocument.Name,
            Instagram = drawingDocument.Instagram,
            Type = (InspirationTypes) drawingDocument.Type,
            Twitter = drawingDocument.Twitter,
            YouTube = drawingDocument.YouTube,
            Twitch = drawingDocument.Twitch,
            Pinterest = drawingDocument.Pinterest,
        };
    }

    public IInspirationDocument ConvertToDocument(InspirationModel drawing)
    {
        return new InspirationMongoDocument
        {
            Id = drawing.Id,
            Name = drawing.Name,
            Instagram = drawing.Instagram,
            Twitter = drawing.Twitter,
            Type = (int) drawing.Type,
            YouTube = drawing.YouTube,
            Twitch = drawing.Twitch,
            Pinterest = drawing.Pinterest,
        };
    }
}
