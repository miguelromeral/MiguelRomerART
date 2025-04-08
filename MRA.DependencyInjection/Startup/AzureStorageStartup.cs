using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.Services.AzureStorage;

namespace MRA.DependencyInjection.Startup;

public static class AzureStorageStartup
{
    public static void AddCustomAzureStorage(this IServiceCollection services)
    {
        services.AddSingleton<IAzureStorageService, AzureStorageService>();
    }
}
