using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing.Details;

public class DrawingControllerDetailsTests : DrawingControllerDetailsBaseTests
{
    public DrawingControllerDetailsTests() : base(onlyIfVisible: true, updateViews: true, useCache: false) { }

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
            Visible = true,
            UrlBase = "https://my.url.com",
            Path = "/image.png",
            PathThumbnail = "/image_tn.png",
            Filter = (int) DrawingFilterTypes.SamsungGalaxy,
            Date = "2025-01-12",
            DateObject = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc),
            Time = 90,
            Views = 10,
            Likes = 5000,
            SpotifyUrl = "https://open.spotify.com/track/6xq3Bd7MvZVa7pda9tC4MW",
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
        Assert.Equal(expected.Id, response.Id);
        Assert.Equal(expected.GetId(), response.GetId());
        Assert.Equal(expected.Name, response.Name);
        Assert.Equal(expected.ModelName, response.ModelName);
        Assert.Equal(expected.ToString(), response.ToString());

        Assert.True(response.Visible);

        Assert.Equal(expected.Visible, response.Visible);

        Assert.Equal("https://my.url.com/image.png", response.Url);
        Assert.Equal("https://my.url.com/image_tn.png", response.UrlThumbnail);
        Assert.Equal("Samsung Galaxy", response.FilterName);
        Assert.Equal("12 enero 2025", response.FormattedDate);
        Assert.Equal("1h 30min", response.TimeHuman);
        Assert.Equal("10", response.ViewsHuman);
        Assert.Equal("5.0 k", response.LikesHuman);
        Assert.Equal("6xq3Bd7MvZVa7pda9tC4MW", response.SpotifyTrackId);
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
        return await _controller.Details(drawingId);
    }
}
