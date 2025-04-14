using Microsoft.Extensions.Caching.Memory;
using MRA.Infrastructure.Settings;
using System.Reflection;

namespace MRA.Services.Cache
{
    public class CacheServiceBase
    {
        internal readonly IMemoryCache _cache;
        internal readonly AppSettings _appSettings;

        public CacheServiceBase(AppSettings appSettings, IMemoryCache cache)
        {
            _cache = cache;
            _appSettings = appSettings;
        }

        public void CleanCacheItem(string item)
        {
            _cache.Remove(item);
        }
        public void CleanAllCache()
        {
            Clear(_cache);
        }

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

        public T GetOrSetFromCache<T>(string cacheKey, Func<T> getDataFunc)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedData))
            {
                return cachedData;
            }

            var data = getDataFunc();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds( _appSettings.Cache.RefreshSeconds)
            };

            _cache.Set(cacheKey, data, cacheEntryOptions);

            return data;
        }

        public async Task<T> GetOrSetFromCacheAsync<T>(string cacheKey, Func<Task<T>> getDataFunc, bool useCache)
        {
            if (!useCache || _cache == null)
            {
                return await getDataFunc();
            }

            if (_cache.TryGetValue(cacheKey, out T cachedData))
            {
                return cachedData;
            }

            var data = await getDataFunc();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_appSettings.Cache.RefreshSeconds)
            };

            _cache.Set(cacheKey, data, cacheEntryOptions);

            return data;
        }

    }
}
