using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Options;
using System.Text;

namespace MRA.DependencyInjection.Startup;

public static class ConfigurationStartup
{
    public static void AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<AppConfiguration>(options =>
        //{
        //    options.Jwt = configuration.GetSection("Jwt").Get<JwtOptions>();
        //    options.Administrator = configuration.GetSection("Administrator").Get<AdministratorOptions>();
        //});

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            var appConfig = new AppConfiguration
            {
                AzureStorage = config.GetSection("AzureStorage").Get<AzureStorageOptions>(),
                AzureKeyVault = config.GetSection("AzureKeyVault").Get<AzureKeyVaultOptions>(),
                Administrator = config.GetSection("Administrator").Get<AdministratorOptions>(),
                Jwt = config.GetSection("Jwt").Get<JwtOptions>(),
                Cache = config.GetSection("Cache").Get<CacheOptions>()
            };

            return appConfig;
        });
    }
}
