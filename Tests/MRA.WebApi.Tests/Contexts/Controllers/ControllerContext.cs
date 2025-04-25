using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MRA.Services.Models.Collections;
using MRA.Services;
using Microsoft.AspNetCore.Mvc;
using MRA.Services.Models.Drawings;
using MRA.Services.Storage;

namespace MRA.UnitTests.Contexts.Controllers;

public class ControllerContext<T> where T : ControllerBase
{
    public ServiceProvider ServiceProvider { get; set; }
    public Mock<IAppService> MockAppService { get; set; }
    public Mock<IDrawingService> MockDrawingService { get; set; }
    public Mock<ICollectionService> MockCollectionService { get; set; }
    public Mock<IStorageService> MockStorageService { get; set; }
    public Mock<ILogger<T>> MockLogger { get; set; }
}
