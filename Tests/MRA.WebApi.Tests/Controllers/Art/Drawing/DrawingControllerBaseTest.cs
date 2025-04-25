using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.Services;
using MRA.Services.Models.Drawings;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Tests.Contexts.Controllers;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing;

public class DrawingControllerBaseTest
{

    protected readonly ServiceProvider _serviceProvider;
    protected readonly Mock<IAppService> _mockAppService;
    protected readonly Mock<IDrawingService> _mockDrawingService;
    protected readonly DrawingController _controller;

    protected DrawingControllerBaseTest()
    {
        var context = ControllerContextFactory.Create<DrawingController>();

        _serviceProvider = context.ServiceProvider;
        _mockAppService = context.MockAppService;
        _mockDrawingService = context.MockDrawingService;

        _controller = _serviceProvider.GetRequiredService<DrawingController>();
    }
}
