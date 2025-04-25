using Microsoft.AspNetCore.Mvc;
using MRA.DTO.Models;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Models.Responses.Errors.Drawings;
using MRA.DTO.Exceptions;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing.Details;

public abstract class DrawingControllerDetailsBaseTests : DrawingControllerBaseTest
{
    protected readonly bool onlyIfVisible;
    protected readonly bool updateViews;
    protected readonly bool useCache;

    protected DrawingControllerDetailsBaseTests(bool onlyIfVisible, bool updateViews, bool useCache)
    {
        this.onlyIfVisible = onlyIfVisible;
        this.updateViews = updateViews;
        this.useCache = useCache;
    }

    protected async Task Base_Details_Ok_DrawingExists()
    {
        var expectedDrawing = Generate_Ok_Drawing();

        var result = await MockDrawingDetails(expectedDrawing.Id, expectedDrawing);

        var response = result.Assert_OkObjectResult();

        Assert_Details_Ok(expectedDrawing, response);
    }

    protected abstract DrawingModel Generate_Ok_Drawing();

    protected abstract Task<ActionResult<DrawingModel>> MockDrawingDetails(string drawingId, DrawingModel expectedDrawing);
    protected abstract void Assert_Details_Ok(DrawingModel expected, DrawingModel response);


    public async Task Base_Details_Error_NotFound()
    {
        var drawingId = "non-existent-id";

        var result = await MockDrawingDetailsNotFound(drawingId);

        result
            .Assert_NotFoundResult()
            .Assert_ErrorResponse(DrawingNotFoundException.CustomMessage(drawingId));
    }

    protected abstract Task<ActionResult<DrawingModel>> MockDrawingDetailsNotFound(string drawingId);


    public async Task Base_Details_Error_InternalError()
    {
        var drawingId = "non-existent-id";
        var expectedError = DrawingDetailsErrorMessages.InternalServer(drawingId);

        var result = await MockDrawingDetailsInternalError(drawingId);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(expectedError);
    }

    protected abstract Task<ActionResult<DrawingModel>> MockDrawingDetailsInternalError(string drawingId);
}
