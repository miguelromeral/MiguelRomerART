using Microsoft.Extensions.DependencyInjection;
using MRA.Services.Firebase.RemoteConfig;
using MRA.Infrastructure.Database;
using MRA.Infrastructure.Firestore;
using MRA.DTO.Models;
using MRA.Infrastructure.Firestore.Documents;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Documents;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Inspirations;
using MRA.DTO.Firebase.Converters;
using MRA.DTO.Firebase.Converters.Interfaces;

namespace MRA.DependencyInjection.Startup;

public static class DatabaseStartup
{
    public static void AddCustomDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, FirestoreDatabase>();

        services.AddSingleton<IFirestoreDocumentConverter<CollectionModel, CollectionDocument>,CollectionFirebaseConverter>();
        services.AddSingleton<IFirestoreDocumentConverter<DrawingModel, DrawingDocument>, DrawingFirebaseConverter>();
        services.AddSingleton<IFirestoreDocumentConverter<InspirationModel, InspirationDocument>, InspirationFirebaseConverter>();

        services.AddSingleton<ICollectionService, CollectionService>();
        services.AddSingleton<IDrawingService, DrawingService>();
        services.AddSingleton<IInspirationService, InspirationService>();

        services.AddSingleton<IRemoteConfigService, RemoteConfigService>();
    }
}
