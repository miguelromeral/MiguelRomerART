using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Models;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Models.Responses.Errors;
using MRA.DTO.Exceptions.Collections;

namespace MRA.UnitTests.Controllers.Art.Collection;


public abstract class CollectionControllerTestsDetailsBase : CollectionControllerTestsBase
{
    protected async Task Base_Details_Ok_CollectionExists()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var expectedCollection = Generate_Ok_Collection();

        var result = await MockCollectionDetails(controller, expectedCollection.Id, expectedCollection);
        
        var response = result.Assert_OkObjectResult();

        Assert_Details_Ok(expectedCollection, response);
    }
    
    protected abstract CollectionModel Generate_Ok_Collection();

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetails(
        CollectionController controller, string collectionId, CollectionModel expectedCollection);
    protected abstract void Assert_Details_Ok(CollectionModel expectedCollection, CollectionResponse response);


    public async Task Base_Details_Error_NotFound()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "non-existent-id";

        var result = await MockCollectionDetailsNotFound(controller, collectionId);

        result
            .Assert_NotFoundResult()
            .Assert_ErrorResponse(CollectionNotFoundException.ErrorMessage(collectionId));
    }

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetailsNotFound(CollectionController controller, string collectionId);


    public async Task Base_Details_Error_InternalError()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var collectionId = "non-existent-id";
        var expectedError = ErrorMessages.Collection.FetchDetails.InternalServer(collectionId);

        var result = await MockCollectionDetailsInternalError(controller, collectionId);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(expectedError);
    }

    protected abstract Task<ActionResult<CollectionResponse>> MockCollectionDetailsInternalError(CollectionController controller, string collectionId);
}
