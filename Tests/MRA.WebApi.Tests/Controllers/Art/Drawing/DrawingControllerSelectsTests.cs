using Moq;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.ViewModels.Art.Select;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Models.Responses.Errors.Drawings;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing;

public class DrawingControllerSelectsTests : DrawingControllerBaseTest
{
    [Fact]
    public async Task Products_Ok()
    {
        var expectedProducts = new List<ProductListItem>()
        {
            new ProductListItem("The Last Of Us", DrawingProductTypes.Videogame),
            new ProductListItem("MR ROBOT", DrawingProductTypes.ActorActress),
        };

        _mockDrawingService.Setup(s => s.GetProductsAsync()).ReturnsAsync(expectedProducts);

        var result = await _controller.Products();

        var response = result.Assert_OkObjectResult();
        Assert.Equal(expectedProducts, response);
    }

    [Fact]
    public async Task Products_Error_InternalServer()
    {
        _mockDrawingService
            .Setup(s => s.GetProductsAsync())
            .ThrowsAsync(new Exception());

        var result = await _controller.Products();

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(DrawingProductErrorMessages.InternalServer);
    }

    [Fact]
    public async Task Characters_Ok()
    {
        var expectedCharacters = new List<CharacterListItem>()
        {
            new CharacterListItem("Ellie Williams", DrawingProductTypes.Videogame),
            new CharacterListItem("Elliot Alderson", DrawingProductTypes.ActorActress),
        };

        _mockDrawingService.Setup(s => s.GetCharactersAsync()).ReturnsAsync(expectedCharacters);

        var result = await _controller.Characters();

        var response = result.Assert_OkObjectResult();
        Assert.Equal(expectedCharacters, response);
    }

    [Fact]
    public async Task Characters_Error_InternalServer()
    {
        _mockDrawingService
            .Setup(s => s.GetCharactersAsync())
            .ThrowsAsync(new Exception());

        var result = await _controller.Characters();

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(DrawingCharactersErrorMessages.InternalServer);
    }

    [Fact]
    public async Task Models_Ok()
    {
        var expectedModels = new List<ModelListItem>()
        {
            new ModelListItem("Aitana Ocaña"),
            new ModelListItem("Emma Stone")
        };
        var expectedResponse = expectedModels.Select(x => x.ModelName).ToList();

        _mockDrawingService.Setup(s => s.GetModelsAsync()).ReturnsAsync(expectedModels);

        var result = await _controller.Models();

        var response = result.Assert_OkObjectResult();
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async Task Models_Error_InternalServer()
    {
        _mockDrawingService
            .Setup(s => s.GetModelsAsync())
            .ThrowsAsync(new Exception());

        var result = await _controller.Models();

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(DrawingModelsErrorMessages.InternalServer);
    }
}
