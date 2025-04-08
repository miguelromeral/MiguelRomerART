using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Configuration;
using MRA.DTO.Options;

namespace MRA.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddSingleton(sp =>
        //{
        //    var config = sp.GetRequiredService<IConfiguration>();

        //    var appConfig = new AppConfiguration
        //    {
        //        AzureStorage = config.GetSection("AzureStorage").Get<AzureStorageOptions>(),
        //        AzureKeyVault = config.GetSection("AzureKeyVault").Get<AzureKeyVaultOptions>(),
        //        Administrator = config.GetSection("Administrator").Get<AdministratorOptions>(),
        //        Jwt = config.GetSection("Jwt").Get<JwtOptions>()
        //    };

        //    return appConfig;
        //});
        services.AddCustomConfiguration(configuration);


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