using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public class BaseCacheService
    {
        private readonly IMemoryCache _cache;

        public BaseCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void CleanCacheItem(string item)
        {
            _cache.Remove(item);
        }

        public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, TimeSpan cacheDuration)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedData))
            {
                return cachedData;
            }

            var data = await getDataFunc();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            _cache.Set(cacheKey, data, cacheEntryOptions);

            return data;
        }
    }
}
