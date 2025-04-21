using Microsoft.Extensions.Caching.Memory;
using MRA.Infrastructure.Settings;
using System.Reflection;

namespace MRA.Infrastructure.Cache;

public class MicrosoftCacheProvider : ICacheProvider
{
    internal readonly IMemoryCache _cache;
    internal readonly AppSettings _appSettings;

    public MicrosoftCacheProvider(AppSettings appSettings, IMemoryCache cache)
    {
        _cache = cache;
        _appSettings = appSettings;
    }

    public void ClearCacheItem(string item)
    {
        _cache.Remove(item);
    }

    public void Clear()
    {
        if (_cache == null)
        {
            throw new ArgumentNullException("Memory cache must not be null");
        }
        else if (_cache is MemoryCache memCache)
        {
            memCache.Compact(1.0);
            return;
        }
        else
        {
            MethodInfo clearMethod = _cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            if (clearMethod != null)
            {
                clearMethod.Invoke(_cache, null);
                return;
            }
            else
            {
                PropertyInfo prop = _cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                if (prop != null)
                {
                    object innerCache = prop.GetValue(_cache);
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

        throw new InvalidOperationException("Unable to clear memory cache instance of type " + _cache.GetType().FullName);
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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_appSettings.Cache.RefreshSeconds)
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
