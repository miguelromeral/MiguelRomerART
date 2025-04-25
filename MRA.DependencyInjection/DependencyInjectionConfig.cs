using Microsoft.Extensions.DependencyInjection;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Configuration;

namespace MRA.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomConfiguration();

        services.AddCustomInfrastructure();
        services.AddCustomServices();

        return services;
    }
}