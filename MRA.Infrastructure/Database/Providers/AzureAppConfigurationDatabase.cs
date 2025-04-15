using Microsoft.Extensions.Configuration;
using MRA.Infrastructure.Settings;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using MRA.Infrastructure.RemoteConfig;

namespace MRA.Infrastructure.Database.Providers;

public class AzureAppConfigurationDatabase : IRemoteConfigDatabase
{
    private readonly AppSettings _appSettings;
    private readonly IConfigurationRoot _configuration;
    private readonly ILogger<AzureAppConfigurationDatabase> _logger;

    public AzureAppConfigurationDatabase(
        AppSettings appConfig,
        ILogger<AzureAppConfigurationDatabase> logger
        )
    {
        _logger = logger;
        _appSettings = appConfig;
        try
        {
            _configuration = new ConfigurationBuilder()
                .AddAzureAppConfiguration(options =>
                {
                    options.Connect(_appSettings.RemoteConfig.ConnectionString)
                           .Select(KeyFilter.Any);
                })
                .Build();
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Error when initializing Azure App Configuration");
        }
    }

    public T GetValue<T>(RemoteConfigSetting<T> remoteConfig)
    {
        try
        {
            return _configuration.GetValue(remoteConfig.Key.ToString(), remoteConfig.DefaultValue);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error when getting Azure App Configuration with key '{Key}'. Returning default value instead.", remoteConfig.Key);
            return remoteConfig.DefaultValue;
        }
    }
}
