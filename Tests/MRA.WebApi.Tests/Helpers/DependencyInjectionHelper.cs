using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MRA.Services.Models.Collections;
using MRA.Services;
using MRA.WebApi.Controllers.Art;
using System;
using MRA.UnitTests.Contexts.Controllers;

namespace MRA.UnitTests.Helpers;

public static class DependencyInjectionHelper
{
    public static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // Mock de ILogger
        var mockLogger = new Mock<ILogger<CollectionController>>();
        services.AddSingleton(mockLogger.Object);

        // Mock de IAppService
        var mockAppService = new Mock<IAppService>();
        services.AddSingleton(mockAppService.Object);

        // Mock de ICollectionService
        var mockCollectionService = new Mock<ICollectionService>();
        services.AddSingleton(mockCollectionService.Object);

        // Registrar el controlador
        services.AddScoped<CollectionController>();

        return services.BuildServiceProvider();
    }

    public static ControllerTestContext BuildControllerProviders()
    {
        var mockAppService = new Mock<IAppService>();
        var mockCollectionService = new Mock<ICollectionService>();
        var mockLogger = new Mock<ILogger<CollectionController>>();

        var services = new ServiceCollection();
        services.AddSingleton(mockAppService.Object);
        services.AddSingleton(mockCollectionService.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddScoped<CollectionController>();

        var serviceProvider = services.BuildServiceProvider();

        return new ControllerTestContext
        {
            ServiceProvider = serviceProvider,
            MockAppService = mockAppService,
            MockCollectionService = mockCollectionService,
            MockLogger = mockLogger
        };
    }
}
