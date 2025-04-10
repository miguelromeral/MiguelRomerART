using MRA.Infrastructure.Configuration;
using MRA.DTO.Firebase.Converters;
using MRA.DTO.Models;
using MRA.Infrastructure.Database;
using MRA.Infrastructure.Firestore.Documents;
using MRA.Services.Models.Documents;

namespace MRA.Services.Models.Inspirations;

public class InspirationService : DocumentModelService<InspirationModel, InspirationDocument>, IInspirationService
{
    public InspirationService(
        AppConfiguration appConfig,
        IFirestoreDocumentConverter<InspirationModel, InspirationDocument> converter,
        IDocumentsDatabase db) 
        : base(collectionName: appConfig.Firebase.CollectionInspirations, converter, db)
    {
    }

    public async Task<IEnumerable<InspirationModel>> GetAllInspirationsAsync()
    {
        return await GetAllAsync();
    }
}
