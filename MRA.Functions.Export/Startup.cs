
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;
using MRA.DTO.Configuration;
using MRA.DTO.Configuration.Options;
using System;


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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.Local.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();
            builder.Services.AddSingleton<IConfiguration>(configuration);

            if (context.EnvironmentName == "Production")
                configurationBuilder.ConfigureKeyVault(configuration);

            builder.Services.AddDependencyInjectionServices(configuration);
        }
    }
}
