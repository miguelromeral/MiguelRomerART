using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        var env = context.HostingEnvironment.EnvironmentName;

        configBuilder
            .SetBasePath(context.HostingEnvironment.ContentRootPath)
            .AddCustomAppSettingsFiles(env, env == "Development")
            .AddEnvironmentVariables();

        if (env == "Production")
        {
            var configuration = configBuilder.Build();
            configBuilder.ConfigureKeyVault(configuration);
        }
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddSingleton(configuration);
        services.AddDependencyInjectionServices(configuration);
    })
    .Build();

host.Run();