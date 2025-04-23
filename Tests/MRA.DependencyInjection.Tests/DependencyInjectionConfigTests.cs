
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.Infrastructure.Cache;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Infrastructure.Excel;
using MRA.Infrastructure.RemoteConfig;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Storage;
using MRA.Infrastructure.Storage.Connection;
using MRA.Infrastructure.UserInput;
using MRA.Services;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Import;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Inspirations;
using MRA.Services.RemoteConfig;
using MRA.Services.Storage;
using MRA.Services.UserInput;

namespace MRA.DependencyInjection.Tests;

public class DependencyInjectionConfigTests
{
    [Fact]
    public void AddDependencyInjectionServices_RegistersAllServices()
    {
        var services = new ServiceCollection();
        IConfigurationRoot configuration = GenerateMockConfiguration();

        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddDependencyInjectionServices(configuration);

        var provider = services.BuildServiceProvider();
        Assert_AppSettings(provider);
        Assert_Infrastructure(provider);
        Assert_Services(provider);
    }

    private static void Assert_Infrastructure(ServiceProvider provider)
    {
        Assert.NotNull(provider.GetService<IDocumentsDatabase>());

        Assert.NotNull(provider.GetService<IAzureStorageConnection>());
        Assert.NotNull(provider.GetService<IStorageProvider>());

        Assert.NotNull(provider.GetService<IUserInputProvider>());
        Assert.NotNull(provider.GetService<IExcelProvider>());
        Assert.NotNull(provider.GetService<IRemoteConfigDatabase>());

        Assert.NotNull(provider.GetService<IMemoryCache>());
        Assert.NotNull(provider.GetService<ICacheProvider>());
    }

    private static void Assert_Services(ServiceProvider provider)
    {
        Assert.NotNull(provider.GetService<IDocumentsDatabase>());

        Assert.NotNull(provider.GetService<IExcelService>());
        Assert.NotNull(provider.GetService<IStorageService>());
        Assert.NotNull(provider.GetService<IRemoteConfigService>());

        Assert.NotNull(provider.GetService<IImportService>());
        Assert.NotNull(provider.GetService<IUserInputService>());

        Assert.NotNull(provider.GetService<ICollectionService>());
        Assert.NotNull(provider.GetService<IDrawingService>());
        Assert.NotNull(provider.GetService<IInspirationService>());

        Assert.NotNull(provider.GetService<IAppService>());
    }

    private static void Assert_AppSettings(ServiceProvider provider)
    {
        var appSettings = provider.GetService<AppSettings>();
        Assert.NotNull(appSettings);

        Assert.Equal("name=value", appSettings.AzureStorage.ConnectionString);
        Assert.Equal("azure-container", appSettings.AzureStorage.BlobStorageContainer);
        Assert.Equal("https://azure-container/", appSettings.AzureStorage.BlobPath);
        Assert.Equal("/Export", appSettings.AzureStorage.ExportLocation);
    }

    private static IConfigurationRoot GenerateMockConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                #region Logging
                { "Logging:LogLevel:Default", "Warning" },
                { "Logging:LogLevel:Microsoft.AspNetCore", "Warning" },
                { "Logging:LogLevel:MRA.WebApi.Controllers", "Warning" },
                { "Logging:LogLevel:MRA.Services", "Warning" },
                { "Logging:LogLevel:MRA.Infrastructure", "Warning" },
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "Warning" },
                #endregion

                { "MRALogger:DateFormat", "HH:mm:ss" },

                #region Azure Storage
                { "AzureStorage:ConnectionString", "name=value" },
                { "AzureStorage:BlobStorageContainer", "azure-container" },
                { "AzureStorage:BlobPath", "https://azure-container/" },
                { "AzureStorage:ExportLocation", "/Export" },
                #endregion

                { "AzureKeyVault:URL", "https://azure-vault/" },

                { "Firebase:CredentialsPath", ".\\credentials\\path" },
                { "Firebase:ProjectID", "projectId" },
                { "Firebase:Environment", "production" },

                { "Database:Name", "" },
                { "Database:Collections:Drawings", "drawings" },
                { "Database:Collections:Inspirations", "inspirations" },
                { "Database:Collections:Collections", "collections" },
                { "Database:Collections:Experience", "experience" },

                { "Database:Drawings:Tags:Delete:0", "a" },
                { "Database:Drawings:Tags:Delete:1", "un" },
                { "Database:Drawings:Tags:Delete:2", "unas" },

                { "Database:Drawings:Tags:Replace:á", "a" },
                { "Database:Drawings:Tags:Replace:é", "e" },

                { "RemoteConfig:ConnectionString", "remote-config-connection-string" },
                { "RemoteConfig:DefaultValues:Popularity:Critic", "1" },
                { "RemoteConfig:DefaultValues:Popularity:Date", "2" },
                { "RemoteConfig:DefaultValues:Popularity:Favorite", "3" },
                { "RemoteConfig:DefaultValues:Popularity:Months", "4" },
                { "RemoteConfig:DefaultValues:Popularity:Popular", "5" },

                { "AzureCosmosDb:ConnectionString", "azure-cosmos-connection-string" },
                { "AzureCosmosDb:DatabaseName", "azure-database" },
                { "AzureCosmosDb:TimeoutSeconds", "30" },

                { "AllowedHosts", "*" },

                { "Cache:RefreshSeconds", "3600" },

                { "Administrator:User", "MiguelRomeral" },
                { "Administrator:Password", "secret-password" },

                { "Jwt:Key", "secret-key" },
                { "Jwt:Issuer", "MiguelRomeral" },
                { "Jwt:Audience", "Audience" },

                { "EPPlus:ExcelPackage:LicenseContext", "NonCommercialPersonal:MiguelRomeral" },
                { "EPPlus:File:Name", "FirestoreDrawings" },
                { "EPPlus:File:DateFormat", "yyyyMMdd_HHmm" },
                { "EPPlus:File:Extension", "xlsx" },

                { "Commands:UpdateEverythingFromExcel", "true" }
            }.ToDictionary(kvp => kvp.Key, kvp => (string?)kvp.Value))
            .Build();
    }
}
