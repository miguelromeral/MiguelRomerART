using Microsoft.Extensions.DependencyInjection;
using MRA.Services.Firebase.RemoteConfig;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Inspirations;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Infrastructure.Database.Providers;

namespace MRA.DependencyInjection.Startup;

public static class FirebaseStartup
{
    public static void AddCustomFirestoreDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, FirestoreDatabase>();
    }

    public static void AddCustomRemoteConfig(this IServiceCollection services)
    {
        services.AddSingleton<IRemoteConfigService, RemoteConfigService>();
    }
}
