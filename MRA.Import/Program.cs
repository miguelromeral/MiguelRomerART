using Microsoft.Extensions.Configuration;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MRA.Services.Import;
using MRA.DependencyInjection;


using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug);
});

var logger = loggerFactory.CreateLogger<Program>();

try
{
    var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddCustomAppSettingsFiles("Development", isDevelopment: true);
    var configuration = configurationBuilder.Build();

    var services = new ServiceCollection();
    services.AddLogging();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug);
    });
    services.AddDependencyInjectionServices(configuration);
    var serviceProvider = services.BuildServiceProvider();

    var importService = serviceProvider.GetRequiredService<IImportService>();
    await importService.Import();
}
catch (Exception ex)
{
    logger.LogError(ex, "Ha ocurrido un error al importar.");
}

Console.WriteLine("Pulse cualquier tecla para continuar");
Console.ReadKey();