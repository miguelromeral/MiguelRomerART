using MRA.Services.Logger;

namespace MRA.WebApi.Startup;

public static class LogginStartup
{
    public static void AddCustomLogging(this ILoggingBuilder logging, IConfiguration configuration, IHostEnvironment environment)
    {
        logging.ClearProviders();
        
        logging.AddConsole();
        if(environment.IsDevelopment())
            logging.AddDebug();

        logging.AddProvider(new MRLoggerProvider(configuration));
    }
}
