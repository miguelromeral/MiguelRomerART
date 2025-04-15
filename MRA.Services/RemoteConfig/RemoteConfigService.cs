using Microsoft.Extensions.Caching.Memory;
using MRA.Infrastructure.RemoteConfig;
using MRA.Infrastructure.Settings;
using MRA.Services.Cache;

namespace MRA.Services.RemoteConfig;

public class RemoteConfigService : CacheServiceBase, IRemoteConfigService
{
    private List<object> _keys;

    private readonly IRemoteConfigDatabase _remoteConfig;

    public RemoteConfigService(
            IMemoryCache cache, 
            AppSettings appSettings, 
            IRemoteConfigDatabase remoteConfigDatabase) : base(appSettings, cache)
    {
        _remoteConfig = remoteConfigDatabase;
        RegisterRemoteConfig();
    }

    private void RegisterRemoteConfig()
    {
        _keys = new List<object>()
        {
            new RemoteConfigSetting<double>(RemoteConfigKey.Popularity_Date, _appSettings.RemoteConfig.DefaultValues.Popularity.Date),
            new RemoteConfigSetting<int>(RemoteConfigKey.Popularity_Months, _appSettings.RemoteConfig.DefaultValues.Popularity.Months),
            new RemoteConfigSetting<double>(RemoteConfigKey.Popularity_Critic, _appSettings.RemoteConfig.DefaultValues.Popularity.Critic),
            new RemoteConfigSetting<double>(RemoteConfigKey.Popularity_Popular, _appSettings.RemoteConfig.DefaultValues.Popularity.Popular),
            new RemoteConfigSetting<double>(RemoteConfigKey.Popularity_Favorite, _appSettings.RemoteConfig.DefaultValues.Popularity.Favorite),
        };
    }

    public double GetPopularityDate() => 
        GetOrSetFromCache(RemoteConfigKey.Popularity_Date.ToString(), () => _remoteConfig.GetValue(GetKey<double>(RemoteConfigKey.Popularity_Date)));
    public int GetPopularityMonths() =>
        GetOrSetFromCache(RemoteConfigKey.Popularity_Date.ToString(), () => _remoteConfig.GetValue(GetKey<int>(RemoteConfigKey.Popularity_Months)));
    public double GetPopularityCritic() =>
        GetOrSetFromCache(RemoteConfigKey.Popularity_Date.ToString(), () => _remoteConfig.GetValue(GetKey<double>(RemoteConfigKey.Popularity_Critic)));
    public double GetPopularityPopular() =>
        GetOrSetFromCache(RemoteConfigKey.Popularity_Date.ToString(), () => _remoteConfig.GetValue(GetKey<double>(RemoteConfigKey.Popularity_Popular)));
    public double GetPopularityFavorite() =>
        GetOrSetFromCache(RemoteConfigKey.Popularity_Date.ToString(), () => _remoteConfig.GetValue(GetKey<double>(RemoteConfigKey.Popularity_Favorite)));


    private RemoteConfigSetting<T> GetKey<T>(RemoteConfigKey key)
    {
        var config = _keys.OfType<RemoteConfigSetting<T>>()
                          .FirstOrDefault(k => k.Key == key);

        if (config == null)
            throw new InvalidOperationException($"La clave '{key}' no está registrada.");

        return config;
    }
}
