using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Models;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Services.Models.Documents;

namespace MRA.Services.Tests.Models.Base;

public class TestDocumentModelService : DocumentModelService<InspirationModel, IInspirationDocument>
{
    public TestDocumentModelService(string collectionName, IDocumentMapper<InspirationModel, IInspirationDocument> converter, IDocumentsDatabase db)
        : base(collectionName, converter, db)
    {
    }
}
