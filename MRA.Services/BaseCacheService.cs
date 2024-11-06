using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public class BaseCacheService
    {
        internal readonly IMemoryCache _cache;

        public BaseCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void CleanCacheItem(string item)
        {
            _cache.Remove(item);
        }
        public void CleanAllCache()
        {
            _cache.Clear();
        }


        public T GetOrSet<T>(string cacheKey, Func<T> getDataFunc, TimeSpan cacheDuration)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedData))
            {
                return cachedData;
            }

            var data = getDataFunc();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            _cache.Set(cacheKey, data, cacheEntryOptions);

            return data;
        }

        public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, TimeSpan cacheDuration)
        {
            if (_cache != null)
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
            else
            {
                return await getDataFunc();
            }
        }

    }
}
