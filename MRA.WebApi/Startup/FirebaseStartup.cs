using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using MRA.DTO.Options;

namespace MRA.WebApi.Startup;

public static class FirebaseStartup
{
    public static void AddFirebase(this IServiceCollection services, IConfiguration configuration, ILogger logger, AppConfiguration appConfig)
    {
        logger.LogInformation("Configurando Firebase");

        var firebaseService = new FirestoreService(configuration, appConfig, new FirestoreDatabase(), logger);
        var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseService.ProjectId, firebaseService.CredentialsPath, configuration.GetValue<int>("CacheSeconds"));

        firebaseService.SetRemoteConfigService(remoteConfigService);
        services.AddSingleton(remoteConfigService);
        services.AddSingleton<IFirestoreService>(firebaseService);
    }
}
