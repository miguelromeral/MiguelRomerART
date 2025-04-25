using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Models;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsFullList : CollectionControllerTestsListBase
{
    [Fact]
    public async Task FullList_Ok()
    {
        await Base_List_Ok();
    }
    protected override IEnumerable<CollectionModel> Generate_Ok_CollectionList()
    {
        return new List<CollectionModel>()
        {
            new CollectionModel()
            {
                Id = "c1",
                DrawingIds = ["d1", "d2"],
                Drawings = new List<DrawingModel>()
                {
                    new DrawingModel()
                    {
                        Id = "d1",
                        Visible = true
                    },
                    new DrawingModel()
                    {
                        Id = "d2",
                        Visible = true
                    },
                }
            },
            new CollectionModel()
            {
                Id = "c2",
                DrawingIds = ["d3", "d4"],
                Drawings = new List<DrawingModel>()
                {
                    new DrawingModel()
                    {
                        Id = "d3",
                        Visible = true
                    },
                    new DrawingModel()
                    {
                        Id = "d4",
                        Visible = false
                    }
                }
            },
        };
    }
    protected override async Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionList(CollectionController controller, IEnumerable<CollectionModel> expectedCollections)
    {
        _mockAppService
            .Setup(s => s.GetAllCollectionsAsync(false, It.IsAny<bool>()))
            .ReturnsAsync(expectedCollections);

        return await FetchList(controller);
    }
    protected override void Assert_VisibleDrawings(CollectionModel responseCollection) { }


    [Fact]
    public async Task FullList_Error_InternalServer()
    {
        await Base_List_Error_InternalError();
    }

    protected override async Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionListInternalError(CollectionController controller)
    {
        _mockAppService
            .Setup(s => s.GetAllCollectionsAsync(false, It.IsAny<bool>()))
            .Throws(new Exception());

        return await FetchList(controller);
    }

    private static async Task<ActionResult<IEnumerable<CollectionResponse>>> FetchList(CollectionController controller)
    {
        return await controller.FullList();
    }
}
