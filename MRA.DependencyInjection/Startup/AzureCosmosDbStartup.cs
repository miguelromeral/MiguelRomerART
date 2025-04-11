
using Microsoft.Extensions.DependencyInjection;
using MRA.Infrastructure.Database.Providers;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.DependencyInjection.Startup;

public static class AzureCosmosDbStartup
{
    public static void AddCustomAzureDatabaseSQL(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, AzureCosmosDbDatabase>();
    }

    public static void AddCustomAzureDatabaseMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, MongoDbDatabase>();
    }
}
