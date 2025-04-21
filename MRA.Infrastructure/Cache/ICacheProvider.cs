using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace MRA.Infrastructure.Cache;

public interface ICacheProvider
{
    void ClearCacheItem(string item);

    void Clear();

    T GetOrSetFromCache<T>(string cacheKey, Func<T> getDataFunc);

    Task<T> GetOrSetFromCacheAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, bool useCache);
}
