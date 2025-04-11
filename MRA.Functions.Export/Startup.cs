
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;


[assembly: FunctionsStartup(typeof(MRA.Functions.Export.Startup))]

namespace MRA.Functions.Export
{

    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(context.ApplicationRootPath)
                .AddCustomAppSettingsFiles(context.EnvironmentName, context.EnvironmentName == "Development")
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();
            builder.Services.AddSingleton<IConfiguration>(configuration);

            if (context.EnvironmentName == "Production")
                configurationBuilder.ConfigureKeyVault(configuration);

            builder.Services.AddDependencyInjectionServices(configuration);
        }
    }
}
