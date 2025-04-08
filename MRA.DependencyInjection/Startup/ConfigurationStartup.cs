using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Configuration.Options;
using MRA.DTO.Configuration;
using System.Text;

namespace MRA.DependencyInjection.Startup;

public static class ConfigurationStartup
{
    public static void AddCustomConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            return sp.GetRequiredService<IConfiguration>().GetMRAConfiguration();
        });
    }

    public static AppConfiguration GetMRAConfiguration(this IConfiguration config)
    {
        var appConfig = new AppConfiguration
        {
            AzureStorage = config.GetSection("AzureStorage").Get<AzureStorageOptions>(),
            AzureKeyVault = config.GetSection("AzureKeyVault").Get<AzureKeyVaultOptions>(),
            Administrator = config.GetSection("Administrator").Get<AdministratorOptions>(),
            Jwt = config.GetSection("Jwt").Get<JwtOptions>(),
            Cache = config.GetSection("Cache").Get<CacheOptions>(),
            Firebase = config.GetSection("Firebase").Get<FirebaseOptions>(),
            MRALogger = config.GetSection("MRALogger").Get<MRALoggerOptions>()
        };

        return appConfig;
    }
}
