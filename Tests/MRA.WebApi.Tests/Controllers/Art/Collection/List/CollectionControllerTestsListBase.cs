using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MRA.DTO.Models;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.UnitTests.Controllers.Art.Collection;

public abstract class CollectionControllerTestsListBase : CollectionControllerTestsBase
{
    protected async Task Base_List_Ok()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var expectedCollection = Generate_Ok_CollectionList();

        var result = await MockCollectionList(controller, expectedCollection);

        var response = result.Assert_OkObjectResult();

        Assert_List_Ok(expectedCollection, response);
    }

    protected abstract IEnumerable<CollectionModel> Generate_Ok_CollectionList();

    protected abstract Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionList(
        CollectionController controller, IEnumerable<CollectionModel> expectedCollections);
    
    protected virtual void Assert_List_Ok(IEnumerable<CollectionModel> expectedCollections, IEnumerable<CollectionResponse> response)
    {
        Assert.NotNull(response);
        Assert.Equal(expectedCollections.Count(), response.Count());
        for (int i = 0; i < expectedCollections.Count(); i++)
        {
            var expectedCollection = expectedCollections.ElementAt(i);
            var responseCollection = response.ElementAt(i);

            Assert.Equal(expectedCollection.Id, responseCollection.Id);
            Assert.Equal(expectedCollection.DrawingIds, responseCollection.DrawingIds);

            for (int j = 0; j < expectedCollection.Drawings.Count(); j++)
            {
                Assert.Equal(expectedCollection.Drawings.ElementAt(j).Id, responseCollection.Drawings.ElementAt(j).Id);
            }

            Assert_VisibleDrawings(responseCollection);
        }
    }

    protected virtual void Assert_VisibleDrawings(CollectionModel responseCollection)
    {
        Assert.All(responseCollection.Drawings, drawing =>
        {
            Assert.True(drawing.Visible, $"Visible drawing found: '{drawing.Id}'");
        });
    }

    public async Task Base_List_Error_InternalError()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var expectedError = ErrorMessages.CollectionErrorMessages.FetchList.InternalServer;

        var result = await MockCollectionListInternalError(controller);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(expectedError);
    }

    protected abstract Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionListInternalError(CollectionController controller);
}
