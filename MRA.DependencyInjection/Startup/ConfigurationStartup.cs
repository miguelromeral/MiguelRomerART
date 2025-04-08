using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Options;
using System.Text;

namespace MRA.DependencyInjection.Startup;

public static class ConfigurationStartup
{
    public static void AddCustomConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

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
        });
    }
}
