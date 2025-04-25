using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.Services.Models.Collections;
using MRA.Services;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Tests.Contexts.Controllers;

namespace MRA.UnitTests.Controllers.Art.Collection;

public abstract class CollectionControllerTestsBase
{
    protected readonly ServiceProvider _serviceProvider;
    protected readonly Mock<IAppService> _mockAppService;
    protected readonly Mock<ICollectionService> _mockCollectionService;

    protected CollectionControllerTestsBase()
    {
        var context = ControllerContextFactory.Create<CollectionController>();

        _serviceProvider = context.ServiceProvider;
        _mockAppService = context.MockAppService;
        _mockCollectionService = context.MockCollectionService;
    }
}
