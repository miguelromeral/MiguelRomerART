//using Microsoft.Extensions.Configuration;
//using MRA.Services.Logger;

//namespace MRA.DependencyInjection.Startup;

//public static class LogginStartup
//{
//    public static void AddCustomLogging(this ILoggingBuilder logging, IConfiguration configuration)
//    {
//        logging.ClearProviders();
//        logging.AddConsole();
//        logging.AddDebug();
//        logging.AddProvider(new MRLoggerProvider(configuration));
//    }
//}
