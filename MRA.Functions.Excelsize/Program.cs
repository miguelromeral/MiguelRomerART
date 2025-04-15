using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var env = builder.Environment;

var configurationBuilder = new ConfigurationBuilder()
    .AddConfiguration(builder.Configuration)
    .SetBasePath(env.ContentRootPath)
    .AddCustomAppSettingsFiles(env.EnvironmentName, env.IsDevelopment())
    .AddEnvironmentVariables();

if (env.IsProduction())
{
    var tempConfig = configurationBuilder.Build();
    configurationBuilder.ConfigureKeyVault(tempConfig);
}

var configuration = configurationBuilder.Build();

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddDependencyInjectionServices(configuration);

builder.Build().Run();
