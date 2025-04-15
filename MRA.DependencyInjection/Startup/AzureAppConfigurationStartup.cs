using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MRA.DependencyInjection.Startup;

public static class AzureAppConfigurationStartup
{
    public static void AddCustomAzureAppConfiguration(this IServiceCollection services)
    {
        services.AddAzureAppConfiguration();
    }
}
