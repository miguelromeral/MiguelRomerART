using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Exceptions;
using MRA.DTO.Exceptions.Collections;
using MRA.DTO.Models;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsFullDetails : CollectionControllerTestsDetailsBase
{
    [Fact]
    public async Task FullDetails_Ok_CollectionExists()
    {
        await Base_Details_Ok_CollectionExists();
    }

    protected override CollectionModel Generate_Ok_Collection()
    {
        var collectionDrawingsIds = new List<string> { "1", "2" };
        return new CollectionModel
        {
            Id = "collection_exists",
            Name = "Test Collection",
            Description = "Description",
            Drawings = new List<DrawingModel>()
            {
                new DrawingModel(){ Id = collectionDrawingsIds[0]},
                new DrawingModel(){ Id = collectionDrawingsIds[1]},
            },
            DrawingIds = collectionDrawingsIds
        };
    }

    protected override async Task<ActionResult<CollectionResponse>> MockCollectionDetails(
        CollectionController controller, string collectionId, CollectionModel expectedCollection)
    {
        _mockAppService
            .Setup(s => s.FindCollectionByIdAsync(collectionId, false, true))
            .ReturnsAsync(expectedCollection);

        return await FetchDetails(controller, collectionId);
    }

    protected override void Assert_Details_Ok(CollectionModel expectedCollection, CollectionResponse response)
    {
        Assert.NotNull(response);
        Assert.Equal(expectedCollection.Id, response.Id);
        Assert.Equal(expectedCollection.Name, response.Name);
        Assert.Equal(expectedCollection.Description, response.Description);
        Assert.Equal(expectedCollection.Drawings.Count(), response.Drawings.Count());
        for (int i = 0; i < expectedCollection.Drawings.Count(); i++)
        {
            DrawingModel drawing = expectedCollection.Drawings.ElementAt(i);
            Assert.Equal(expectedCollection.DrawingIds.ElementAt(i), drawing.Id);
        }
    }


    [Fact]
    public async Task FullDetails_Error_NotFound()
    {
        await Base_Details_Error_NotFound();
    }

    protected override async Task<ActionResult<CollectionResponse>> MockCollectionDetailsNotFound(
        CollectionController controller, string collectionId)
    {
        _mockAppService
            .Setup(s => s.FindCollectionByIdAsync(collectionId, false, true))
            .ThrowsAsync(new CollectionNotFoundException(collectionId));

        return await FetchDetails(controller, collectionId);
    }

    [Fact]
    public async Task FullDetails_Error_InternalError()
    {
        await Base_Details_Error_InternalError();
    }

    protected override async Task<ActionResult<CollectionResponse>> MockCollectionDetailsInternalError(CollectionController controller, string collectionId)
    {
        _mockAppService
            .Setup(s => s.FindCollectionByIdAsync(collectionId, false, true))
            .ThrowsAsync(new Exception());

        return await FetchDetails(controller, collectionId);
    }

    private static async Task<ActionResult<CollectionResponse>> FetchDetails(CollectionController controller, string collectionId)
    {
        return await controller.FullDetails(collectionId);
    }
}
