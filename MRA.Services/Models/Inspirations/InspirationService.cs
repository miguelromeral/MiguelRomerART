using MRA.DTO.Models;
using MRA.Services.Models.Documents;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.DTO.Mapper.Interfaces;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.DTO.Mapper;

namespace MRA.Services.Models.Inspirations;

public class InspirationService : DocumentModelService<InspirationModel, IInspirationDocument>, IInspirationService
{
    public InspirationService(
        AppSettings appConfig,
        IDocumentsDatabase db) 
        : base(collectionName: appConfig.Database.Collections.Inspirations, new InspirationMapper(), db)
    {
    }

    public async Task<IEnumerable<InspirationModel>> GetAllInspirationsAsync()
    {
        return await GetAllAsync();
    }
}
