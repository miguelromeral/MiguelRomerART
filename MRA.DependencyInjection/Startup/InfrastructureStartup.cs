using Microsoft.Extensions.DependencyInjection;
using MRA.Infrastructure.Database.Providers;
using MRA.Infrastructure.Storage;
using MRA.Infrastructure.Excel;
using MRA.Infrastructure.RemoteConfig;
using MRA.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Infrastructure.Storage.Connection;
using MRA.Infrastructure.UserInput;

namespace MRA.DependencyInjection.Startup;

public static class InfrastructureStartup
{
    public static void AddCustomInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, MongoDbDatabase>();

        services.AddSingleton<IAzureStorageConnection, AzureStorageConnection>();
        services.AddSingleton<IStorageProvider, AzureStorageProvider>();

        services.AddSingleton<IUserInputProvider, ConsoleProvider>();
        services.AddSingleton<IExcelProvider, EPPlusExcelProvider>();
        services.AddSingleton<IRemoteConfigDatabase, AzureAppConfigurationDatabase>();

        services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
        services.AddSingleton<ICacheProvider, MicrosoftCacheProvider>();
    }
}
