using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Settings.Options;
using MRA.Infrastructure.Settings.Sections;
using static MRA.Infrastructure.Settings.Options.DatabaseSettings;
using static MRA.Infrastructure.Settings.Options.DatabaseSettings.DatabaseDrawingsTagsOptions;

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

    public static AppSettings GetMRAConfiguration(this IConfiguration config)
    {
        var appConfig = new AppSettings
        {
            AzureStorage = config.GetSection("AzureStorage").Get<AzureStorageSettings>(),
            AzureKeyVault = config.GetSection("AzureKeyVault").Get<AzureKeyVaultSettings>(),
            Administrator = config.GetSection("Administrator").Get<AdministratorSettings>(),
            Jwt = config.GetSection("Jwt").Get<JwtSettings>(),
            Cache = config.GetSection("Cache").Get<CacheSettings>(),
            Firebase = config.GetSection("Firebase").Get<FirebaseSettings>(),
            MRALogger = config.GetSection("MRALogger").Get<MRALoggerSettings>(),
            EPPlus = config.GetSection("EPPlus").Get<EPPlusSettings>(),
            AzureCosmosDb = config.GetSection("AzureCosmosDb").Get<AzureCosmosSettings>(),
            Database = config.GetSection("Database").Get<DatabaseSettings>(),
            //Database = new DatabaseSettings
            //{
            //    Name = config["Database:Name"],
            //    Collections = config.GetSection("Database:Collections").Get<DatabaseCollectionsOptions>(),
            //    Drawings = new DatabaseDrawingsOptions
            //    {
            //        Tags = new DatabaseDrawingsTagsOptions
            //        {
            //            Delete = config.GetSection("Database:Drawings:Tags:Delete").Get<List<string>>(),
            //            Replace = config.GetSection("Database:Drawings:Tags:Replace").Get<List<DatabaseDrawingsTagsReplaceOptions>>()
            //        }
            //    }
            //},
            RemoteConfig = config.GetSection("RemoteConfig").Get<RemoteConfigSettings>(),
            Commands = config.GetSection("Commands").Get<CommandsSettings>()
        };

        return appConfig;
    }
}
