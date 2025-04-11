using MRA.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Models.Collections;

public interface ICollectionService
{
    Task<IEnumerable<CollectionModel>> GetAllCollectionsAsync(bool onlyIfVisible);
    Task<CollectionModel> FindCollectionAsync(string id);
    Task<bool> ExistsCollection(string id);
    Task<bool> SaveCollectionAsync(string id, CollectionModel collection);
    Task<bool> DeleteCollection(string id);
}
