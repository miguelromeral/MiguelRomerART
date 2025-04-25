using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.DTO.Models;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsSave : CollectionControllerTestsBase
{
    [Fact]
    public async Task Save_Ok()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var originalCollection = new CollectionModel()
        {
            Id = "test-id",
            Name = "Old Name",
            Description = "Description",
        };
        var requestModel = new SaveCollectionRequest()
        {
            Id = originalCollection.Id,
            Name = "New Name",
            Description = originalCollection.Description
        };
        var expectedCollection = new CollectionModel()
        {
            Id = originalCollection.Id,
            Name = requestModel.Name,
            Description = originalCollection.Description
        };

        _mockCollectionService
            .Setup(s => s.SaveCollectionAsync(originalCollection.Id, originalCollection))
            .ReturnsAsync(true);

        var result = await controller.Save(requestModel.Id, requestModel);

        var response = result.Assert_OkObjectResult();
        Assert.NotNull(response);
        Assert.Equal(expectedCollection.Id, response.Id);
        Assert.Equal(expectedCollection.Name, response.Name);
        Assert.NotEqual(originalCollection.Name, response.Name);
        Assert.Equal(expectedCollection.Description, response.Description);
    }

    [Fact]
    public async Task Save_Error_IdNotProvided()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var requestModel = new SaveCollectionRequest()
        {
            Id = string.Empty,
        };

        var result = await controller.Save(requestModel.Id, requestModel);

        result
            .Assert_BadRequestResult()
            .Assert_ErrorResponse(ErrorMessages.CollectionErrorMessages.Save.IdNotProvided);
    }

    [Fact]
    public async Task Save_Error_InternalServer()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var requestModel = new SaveCollectionRequest()
        {
            Id = "error-server"
        };

        _mockCollectionService
            .Setup(s => s.SaveCollectionAsync(requestModel.Id, It.Is<CollectionModel>(c => c.Id == requestModel.Id)))
            .ThrowsAsync(new Exception());

        var result = await controller.Save(requestModel.Id, requestModel);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(ErrorMessages.CollectionErrorMessages.Save.InternalServer(requestModel.Id));
    }
}
