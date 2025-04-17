using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Controllers.Art;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsExists : CollectionControllerTestsBase
{
    [Fact]
    public async Task Exists_Ok_Found()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";

        _mockCollectionService
            .Setup(s => s.ExistsCollection(collectionId))
            .ReturnsAsync(true);

        var result = await controller.Exists(collectionId);

        var response = result.Assert_OkObjectResult();
        Assert.True(response);
    }

    [Fact]
    public async Task Exists_Error_NotFound()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";

        _mockCollectionService
            .Setup(s => s.ExistsCollection(collectionId))
            .ReturnsAsync(false);

        var result = await controller.Exists(collectionId);

        var response = result.Assert_OkObjectResult();
        Assert.False(response);
    }

    [Fact]
    public async Task Exists_Error_InternalServer()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";
        var expectedError = $"Error when checking collection \"{collectionId}\"";

        _mockCollectionService
            .Setup(s => s.ExistsCollection(collectionId))
            .ThrowsAsync(new Exception());

        var result = await controller.Exists(collectionId);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(expectedError);
    }
}
