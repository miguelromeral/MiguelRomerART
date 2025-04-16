using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.WebApi.Controllers.Art;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsExists : CollectionControllerTestsBase
{
    [Fact]
    public async Task Exists_ReturnsTrue_WhenCollectionExists()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";

        _mockCollectionService
            .Setup(s => s.ExistsCollection(collectionId))
            .ReturnsAsync(true);

        var result = await controller.Exists(collectionId);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;

        Assert.NotNull(okResult);
        Assert.True((bool) okResult.Value);
    }
}
