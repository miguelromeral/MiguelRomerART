using MRA.DTO.Models;
using MRA.Services.Models.Documents;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.DTO.Mapper.Interfaces;
using MRA.Infrastructure.Configuration;
using MRA.Infrastructure.Database.Documents.Interfaces;

namespace MRA.Services.Models.Inspirations;

public class InspirationService : DocumentModelService<InspirationModel, IInspirationDocument>, IInspirationService
{
    public InspirationService(
        AppConfiguration appConfig,
        IDocumentMapper<InspirationModel, IInspirationDocument> converter,
        IDocumentsDatabase db) 
        : base(collectionName: appConfig.Database.Collections.Inspirations, converter, db)
    {
    }

    public async Task<IEnumerable<InspirationModel>> GetAllInspirationsAsync()
    {
        return await GetAllAsync();
    }
}
