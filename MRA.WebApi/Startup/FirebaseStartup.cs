using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;

namespace MRA.WebApi.Startup;

public static class FirebaseStartup
{
    public static void AddFirebase(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation("Configurando Firebase");

        var firebaseService = new FirestoreService(configuration, new FirestoreDatabase(), logger);
        var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseService.ProjectId, firebaseService.CredentialsPath, configuration.GetValue<int>("CacheSeconds"));

        firebaseService.SetRemoteConfigService(remoteConfigService);
        services.AddSingleton(remoteConfigService);
        services.AddSingleton<IFirestoreService>(firebaseService);
    }
}
