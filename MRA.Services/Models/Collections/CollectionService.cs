using MRA.Infrastructure.Settings;
using MRA.DTO.Models;
using MRA.Services.Models.Documents;
using MRA.DTO.Exceptions;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.DTO.Mapper.Interfaces;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.DTO.Exceptions.Collections;
using System.Linq.Expressions;
using MRA.DTO.Mapper;

namespace MRA.Services.Models.Collections;

public class CollectionService : DocumentModelService<CollectionModel, ICollectionDocument>, ICollectionService
{
    public CollectionService(
        AppSettings appConfig,
        IDocumentsDatabase db)
        : base(collectionName: appConfig.Database.Collections.Collections, new CollectionMapper(), db)
    {
    }

    public async Task<IEnumerable<CollectionModel>> GetAllCollectionsAsync(bool onlyIfVisible)
    {
        var collections = await GetAllAsync();
        return collections;
    }

    public async Task<CollectionModel> FindCollectionAsync(string id)
    {
        await CheckIfExistsCollectionAsync(id);

        return await FindAsync(id);
    }

    public async Task<bool> ExistsCollection(string id)
    {
        return await ExistsAsync(id);
    }

    public async Task<bool> SaveCollectionAsync(string id, CollectionModel collection)
    {
        return await SetAsync(id, collection);
    }

    public async Task<bool> DeleteCollection(string id)
    {
        await CheckIfExistsCollectionAsync(id);

        return await DeleteAsync(id);
    }

    private async Task CheckIfExistsCollectionAsync(string id)
    {
        var exists = await ExistsAsync(id);
        if (!exists)
            throw new CollectionNotFoundException(id);
    }

}
