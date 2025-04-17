using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MRA.Services.Models.Collections;
using MRA.Services;
using MRA.WebApi.Controllers.Art;

namespace MRA.UnitTests.Contexts.Controllers;

public class ControllerTestContext
{
    public ServiceProvider ServiceProvider { get; set; }
    public Mock<IAppService> MockAppService { get; set; }
    public Mock<ICollectionService> MockCollectionService { get; set; }
    public Mock<ILogger<CollectionController>> MockLogger { get; set; }
}
