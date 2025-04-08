using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Configuration;
using MRA.DTO.Configuration;
using MRA.Services.AzureStorage;
using MRA.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MRA.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomConfiguration();
        services.AddCustomLogger();

        services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));

        services.AddCustomAzureStorage();
        services.AddCustomFirebase();
        services.AddSingleton<IDrawingService, DrawingService>();

        return services;
    }
}