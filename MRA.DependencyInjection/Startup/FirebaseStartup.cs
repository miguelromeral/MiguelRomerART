using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using Microsoft.Extensions.DependencyInjection;
using MRA.Services.Firebase.RemoteConfig;

namespace MRA.DependencyInjection.Startup;

public static class FirebaseStartup
{
    public static void AddCustomFirebase(this IServiceCollection services)
    {
        services.AddSingleton<IFirestoreDatabase, FirestoreDatabase>();
        services.AddSingleton<IRemoteConfigService, RemoteConfigService>();
        services.AddSingleton<IFirestoreService, FirestoreService>();
    }
}
