using MRA.Infrastructure.Cache;
using MRA.Infrastructure.Settings;

namespace MRA.Services.Cache
{
    public class CacheServiceBase
    {
        internal readonly ICacheProvider _cache;
        internal readonly AppSettings _appSettings;

        public CacheServiceBase(AppSettings appSettings, ICacheProvider cache)
        {
            _cache = cache;
            _appSettings = appSettings;
        }

        public void CleanCacheItem(string item)
        {
            _cache.ClearCacheItem(item);
        }
        public void Clear()
        {
            _cache.Clear();
        }

        public T GetOrSetFromCache<T>(string cacheKey, Func<T> getDataFunc)
        {
            return _cache.GetOrSetFromCache(cacheKey, getDataFunc);
        }

        public async Task<T> GetOrSetFromCacheAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, bool useCache)
        {
            return await _cache.GetOrSetFromCacheAsync(cacheKey, getDataFunc, useCache);
        }
    }
}
