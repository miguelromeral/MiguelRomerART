//using Microsoft.Extensions.Configuration;

//namespace MRA.DependencyInjection.Startup;

//public static class SettingsStartup
//{
//    public static void AddAppSettings(this ConfigurationBuilder builder, string environment)
//    {
//        builder
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//            .AddJsonFile($"appsettings.{environment}.json", optional: true)
//            .AddJsonFile($"appsettings.Local.json", optional: true)
//            .AddEnvironmentVariables();
//    }
//}
