using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Options;
using MRA.Services.AzureStorage;

namespace MRA.DependencyInjection.Startup;

public static class AzureStorageStartup
{
    public static void AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<AzureStorageOptions>(configuration.GetSection("AzureStorage"));
        services.AddSingleton<IAzureStorageService, AzureStorageService>();
    }
}
