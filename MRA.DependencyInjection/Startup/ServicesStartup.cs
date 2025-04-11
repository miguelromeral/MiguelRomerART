using Microsoft.Extensions.DependencyInjection;
using MRA.Services;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Excel;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Inspirations;

namespace MRA.DependencyInjection.Startup;

public static class ServicesStartup
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddCustomAzureStorage();
        services.AddCustomRemoteConfig();

        services.AddSingleton<IExcelService, ExcelService>();
        services.AddSingleton<ICollectionService, CollectionService>();
        services.AddSingleton<IDrawingService, DrawingService>();
        services.AddSingleton<IInspirationService, InspirationService>();

        services.AddSingleton<IAppService, AppService>();
    }
}
