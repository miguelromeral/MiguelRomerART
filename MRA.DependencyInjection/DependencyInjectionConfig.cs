using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Configuration;
using MRA.DTO.Options;
using MRA.Services.AzureStorage;

namespace MRA.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomConfiguration(configuration);

        services.AddAzureStorage(configuration);


        //services.Configure<AppConfiguration>(configuration);

        //services.Configura

        // Agrega aquí todas tus configuraciones de DI
        // Por ejemplo:
        //services.AddScoped<IMiServicio, MiServicio>();
        //services.AddSingleton<IMiSingleton, MiSingleton>();

        // También puedes agregar configuración, AutoMapper, etc.
        // services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}