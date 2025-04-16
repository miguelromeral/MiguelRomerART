using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;
using MRA.UnitTests.Extensions;

namespace MRA.UnitTests.Controllers.Art.Collection.Details;


public abstract class CollectionControllerTestsDetailsBase : CollectionControllerTestsBase
{
    protected async Task Base_Details_Ok_CollectionExists()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var expectedCollection = Generate_Ok_Collection();

        var result = await MockCollectionDetails(controller, expectedCollection.Id, expectedCollection);
        
        OkObjectResult okResult = result.Assert_OkObjectResult();

        Assert_Details_Ok(expectedCollection, okResult);
    }
    
    protected abstract CollectionModel Generate_Ok_Collection();

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetails(
        CollectionController controller, string collectionId, CollectionModel expectedCollection);
    protected abstract void Assert_Details_Ok(CollectionModel expectedCollection, OkObjectResult okResult);


    public async Task Base_Details_Error_NotFound()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "non-existent-id";
        var expectedError = CollectionNotFoundException.CustomMessage(collectionId);

        var result = await MockCollectionDetailsNotFound(controller, collectionId);

        result
            .Assert_NotFoundResult()
            .Assert_NotFoundResponse(expectedError);
    }

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetailsNotFound(CollectionController controller, string collectionId);


    public async Task Base_Details_Error_InternalError()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "non-existent-id";
        var expectedError = $"Error when retrieving collection with ID \"{collectionId}\"";

        var result = await MockCollectionDetailsInternalError(controller, collectionId);

        result
            .Assert_InternalErrorResult()
            .Assert_NotFoundResponse(expectedError);
    }

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetailsInternalError(CollectionController controller, string collectionId);
}
