using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.DTO.Exceptions.Collections;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsDelete : CollectionControllerTestsBase
{

    [Fact]
    public async Task Delete_Ok_Found()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";

        _mockCollectionService
            .Setup(s => s.DeleteCollection(collectionId))
            .ReturnsAsync(true);

        var result = await controller.Delete(collectionId);

        var response = result.Assert_OkObjectResult();
        Assert.True(response);
    }

    [Fact]
    public async Task Delete_Error_NotFound()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "test-id";

        _mockCollectionService
            .Setup(s => s.DeleteCollection(collectionId))
            .Throws(new CollectionNotFoundException(collectionId));

        var result = await controller.Delete(collectionId);

        result
            .Assert_NotFoundResult()
            .Assert_ErrorResponse(CollectionNotFoundException.ErrorMessage(collectionId));
    }

    [Fact]
    public async Task Delete_Error_InternalServer()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var id = "test-id";

        _mockCollectionService
            .Setup(s => s.DeleteCollection(id))
            .ThrowsAsync(new Exception());

        var result = await controller.Delete(id);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(ErrorMessages.CollectionErrorMessages.Delete.InternalServer(id));
    }
}
