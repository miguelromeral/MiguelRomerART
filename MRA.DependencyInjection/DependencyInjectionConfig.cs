using Microsoft.Extensions.DependencyInjection;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Configuration;
using MRA.Services;
using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Excel;

namespace MRA.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomConfiguration();
        services.AddCustomLogger();

        services.AddCustomInfrastructure();
        services.AddCustomServices();

        return services;
    }
}