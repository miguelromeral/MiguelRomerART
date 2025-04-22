using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using MRA.DependencyInjection.Startup;

namespace MRA.DependencyInjection.Tests;

public class AppSettingsStartupTests
{
    [Theory]
    [InlineData("Production", false, new[] { "appsettings.json", "appsettings.Production.json" })]
    [InlineData("Development", true, new[] { "appsettings.json", "appsettings.Development.json", "appsettings.Local.json" })]
    [InlineData("Staging", false, new[] { "appsettings.json", "appsettings.Staging.json" })]
    [InlineData("Staging", true, new[] { "appsettings.json", "appsettings.Staging.json", "appsettings.Local.json" })]
    public void AddCustomAppSettingsFiles_Ok(string environment, bool isDevelopment, string[] expectedFiles)
    {
        var builder = new ConfigurationBuilder();

        builder.AddCustomAppSettingsFiles(environment, isDevelopment);

        var sources = builder.Sources
            .OfType<JsonConfigurationSource>() // Filtrar solo los JSON sources
            .Select(source => source.Path)     // Obtener las rutas de los archivos
            .ToArray();

        Assert.Equal(expectedFiles.Length, sources.Length);
        Assert.All(expectedFiles, file => Assert.Contains(file, sources));
    }
}
