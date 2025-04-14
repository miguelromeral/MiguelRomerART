using Microsoft.Extensions.Configuration;
using MRA.Infrastructure.Settings;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace MRA.DependencyInjection.Startup;

public static class AzureAppConfigurationStartup
{
    //public static IConfigurationBuilder ConfigureAzureAppConfiguration(this IConfigurationBuilder builder)
    //{
    //    builder.AddAzureAppConfiguration(options =>
    //    {
    //        options.Connect("Endpoint=https://<nombre-de-tu-configuración>.azconfig.io;Id=<Id>;Secret=<Secret>")
    //               .Select(KeyFilter.Any, LabelFilter.Null) // Opcional: Filtras claves y etiquetas si lo necesitas
    //               .ConfigureRefresh(refresh =>
    //               {
    //                   refresh.Register("AppSettings:Sentinel", refreshAll: true)
    //                          .SetCacheExpiration(TimeSpan.FromMinutes(5));
    //               });
    //    });

    //    return builder;
    //}

    public static void AddCustomAzureAppConfiguration(this IServiceCollection services)
    {
        services.AddAzureAppConfiguration();
    }
}
