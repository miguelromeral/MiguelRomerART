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
            Clear(_cache);
        }


        /// <summary>
        /// Clear IMemoryCache
        /// </summary>
        /// <param name="cache">Cache</param>
        /// <exception cref="InvalidOperationException">Unable to clear memory cache</exception>
        /// <exception cref="ArgumentNullException">Cache is null</exception>
        private void Clear(IMemoryCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("Memory cache must not be null");
            }
            else if (cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
                return;
            }
            else
            {
                MethodInfo clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                if (clearMethod != null)
                {
                    clearMethod.Invoke(cache, null);
                    return;
                }
                else
                {
                    PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                    if (prop != null)
                    {
                        object innerCache = prop.GetValue(cache);
                        if (innerCache != null)
                        {
                            clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                            if (clearMethod != null)
                            {
                                clearMethod.Invoke(innerCache, null);
                                return;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("Unable to clear memory cache instance of type " + cache.GetType().FullName);
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
