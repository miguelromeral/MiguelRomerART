using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Mapper;
using MRA.DTO.Models;
using MRA.Infrastructure.Database.Providers;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.RemoteConfig;

namespace MRA.DependencyInjection.Startup;

public static class InfrastructureStartup
{
    public static void AddCustomInfrastructure(this IServiceCollection services)
    {
        services.AddCustomAzureDatabaseMongoDb();
        
        services.AddSingleton<IRemoteConfigDatabase, AzureAppConfigurationDatabase>();

        services.AddSingleton<IDocumentMapper<CollectionModel, ICollectionDocument>, CollectionMapper>();
        services.AddSingleton<IDocumentMapper<DrawingModel, IDrawingDocument>, DrawingMapper>();
        services.AddSingleton<IDocumentMapper<InspirationModel, IInspirationDocument>, InspirationMapper>();
    }
}
