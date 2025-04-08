using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MRA.DTO.Logger;

namespace MRA.DependencyInjection.Startup;

public static class LoggerStartup
{
    public static void AddCustomLogger(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, MRLogger>();
    }
}
