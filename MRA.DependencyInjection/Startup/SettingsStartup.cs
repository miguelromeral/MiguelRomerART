//namespace MRA.DependencyInjection.Startup;

//public static class SettingsStartup
//{
//    public static void AddAppSettings(this WebApplicationBuilder builder)
//    {
//        builder.Configuration
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
//            .AddJsonFile($"appsettings.Local.json", optional: true)
//            .AddEnvironmentVariables();
//    }
//}
