using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing.Details;

public class DrawingControllerFullDetailsTests : DrawingControllerDetailsBaseTests
{
    public DrawingControllerFullDetailsTests() : base(onlyIfVisible: false, updateViews: true, useCache: false) { }

    [Fact]
    public async Task Details_Ok_DrawingExists()
    {
        await Base_Details_Ok_DrawingExists();
    }

    protected override DrawingModel Generate_Ok_Drawing()
    {
        return new DrawingModel
        {
            Id = "drawingId",
            Name = "Name",
            ModelName = "ModelName",
            Visible = false,
            Likes = 1200300
        };
    }

    protected override async Task<ActionResult<DrawingModel>> MockDrawingDetails(string drawingId, DrawingModel expectedDrawing)
    {
        _mockAppService
            .Setup(s => s.FindDrawingByIdAsync(drawingId, onlyIfVisible, updateViews, useCache))
            .ReturnsAsync(expectedDrawing);

        return await FetchDetails(drawingId);
    }

    protected override void Assert_Details_Ok(DrawingModel expected, DrawingModel response)
    {
        Assert.NotNull(response);
        Assert.Equal("drawingId", response.Id);
        Assert.Equal("Name", response.Name);
        Assert.Equal("ModelName", response.ModelName);
        Assert.False(response.Visible);

        Assert.Equal(string.Empty, response.SpotifyTrackId);
        Assert.Equal("N/A", response.TimeHuman);
        Assert.Equal("1.2 M", response.LikesHuman);
    }

    [Fact]
    public async Task Details_Error_NotFound()
    {
        await Base_Details_Error_NotFound();
    }

    protected override async Task<ActionResult<DrawingModel>> MockDrawingDetailsNotFound(string drawingId)
    {
        _mockAppService
            .Setup(s => s.FindDrawingByIdAsync(drawingId, onlyIfVisible, updateViews, useCache))
            .ThrowsAsync(new DrawingNotFoundException(drawingId));

        return await FetchDetails(drawingId);
    }

    [Fact]
    public async Task Details_Error_InternalError()
    {
        await Base_Details_Error_InternalError();
    }

    protected override async Task<ActionResult<DrawingModel>> MockDrawingDetailsInternalError(string drawingId)
    {
        _mockAppService
            .Setup(s => s.FindDrawingByIdAsync(drawingId, onlyIfVisible, updateViews, useCache))
            .ThrowsAsync(new Exception());

        return await FetchDetails(drawingId);
    }

    private async Task<ActionResult<DrawingModel>> FetchDetails(string drawingId)
    {
        return await _controller.FullDetails(drawingId);
    }
}
