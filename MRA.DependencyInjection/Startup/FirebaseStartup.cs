using Microsoft.Extensions.DependencyInjection;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Infrastructure.Database.Providers;

namespace MRA.DependencyInjection.Startup;

public static class FirebaseStartup
{
    public static void AddCustomFirestoreDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentsDatabase, FirestoreDatabase>();
    }
}
