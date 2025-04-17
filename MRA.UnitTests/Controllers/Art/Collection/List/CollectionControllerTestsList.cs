using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MRA.DTO.Exceptions.Collections;
using MRA.DTO.Models;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Controllers.Art;
using MRA.WebApi.Models.Responses;
using MRA.WebApi.Models.Responses.Errors;
using Xunit.Sdk;

namespace MRA.UnitTests.Controllers.Art.Collection;

public class CollectionControllerTestsList : CollectionControllerTestsListBase
{

    [Fact]
    public async Task List_Ok()
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
                DrawingIds = ["d3"],
                Drawings = new List<DrawingModel>()
                {
                    new DrawingModel()
                    {
                        Id = "d3",
                        Visible = true
                    }
                }
            },
        };
    }
    protected override async Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionList(CollectionController controller, IEnumerable<CollectionModel> expectedCollections)
    {
        _mockAppService
            .Setup(s => s.GetAllCollectionsAsync(true, It.IsAny<bool>()))
            .ReturnsAsync(expectedCollections);

        return await FetchList(controller);
    }


    [Fact]
    public async Task List_Error_NotVisibleFetched()
    {
        var controller = _serviceProvider.GetRequiredService<CollectionController>();
        var expectedCollectionList = Generate_Ok_CollectionList().ToList();
        
        var drawings = expectedCollectionList[0].Drawings.ToList();
        drawings[0].Visible = false;
        expectedCollectionList[0].Drawings = drawings;

        _mockAppService
            .Setup(s => s.GetAllCollectionsAsync(true, It.IsAny<bool>()))
            .ReturnsAsync(expectedCollectionList);

        var result = await FetchList(controller);

        result
            .Assert_ServiceUnavailable()
            .Assert_ErrorResponse(VisibleCollectionRetrievedException.ErrorMessage);
    }

    [Fact]
    public async Task List_Error_InternalServer()
    {
        await Base_List_Error_InternalError();
    }

    protected override async Task<ActionResult<IEnumerable<CollectionResponse>>> MockCollectionListInternalError(CollectionController controller)
    {
        _mockAppService
            .Setup(s => s.GetAllCollectionsAsync(true, It.IsAny<bool>()))
            .Throws(new Exception());

        return await FetchList(controller);
    }

    private static async Task<ActionResult<IEnumerable<CollectionResponse>>> FetchList(CollectionController controller)
    {
        return await controller.List();
    }
}
