using Google.Api;
using Microsoft.Extensions.Configuration;

namespace MRA.DependencyInjection.Startup;

public static class SettingsStartup
{
    public static IConfigurationBuilder AddAppSettingsFiles(this IConfigurationBuilder builder, string environment)
    {
        builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddJsonFile($"appsettings.Local.json", optional: true);

        return builder;
    }
}
