using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.Services.Models.Collections;
using MRA.Services;
using MRA.UnitTests.Contexts.Controllers;
using Microsoft.Extensions.Logging;
using MRA.Services.Models.Drawings;
using MRA.Services.Storage;

namespace MRA.WebApi.Tests.Contexts.Controllers;

public static class ControllerContextFactory
{
    public static ControllerContext<T> Create<T>() where T : ControllerBase
    {
        var mockAppService = new Mock<IAppService>();
        var mockDrawingService = new Mock<IDrawingService>();
        var mockStorageService = new Mock<IStorageService>();
        var mockCollectionService = new Mock<ICollectionService>();
        var mockLogger = new Mock<ILogger<T>>();

        var services = new ServiceCollection();
        services.AddSingleton(mockAppService.Object);
        services.AddSingleton(mockDrawingService.Object);
        services.AddSingleton(mockCollectionService.Object);
        services.AddSingleton(mockStorageService.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddScoped<T>();

        var serviceProvider = services.BuildServiceProvider();

        return new ControllerContext<T>
        {
            ServiceProvider = serviceProvider,
            MockAppService = mockAppService,
            MockDrawingService = mockDrawingService,
            MockCollectionService = mockCollectionService,
            MockStorageService = mockStorageService,
            MockLogger = mockLogger
        };
    }
}
