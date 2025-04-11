using Google.Api;
using Microsoft.Extensions.Configuration;

namespace MRA.DependencyInjection.Startup;

public static class SettingsStartup
{
    public static IConfigurationBuilder AddCustomAppSettingsFiles(this IConfigurationBuilder builder, string environment, bool isDevelopment)
    {
        builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true);

        if(isDevelopment)
            builder.AddJsonFile($"appsettings.Local.json", optional: true);

        return builder;
    }
}
