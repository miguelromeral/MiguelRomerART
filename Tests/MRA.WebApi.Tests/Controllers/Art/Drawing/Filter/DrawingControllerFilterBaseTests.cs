using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;
using MRA.UnitTests.Extensions;
using MRA.WebApi.Models.Responses.Errors.Drawings;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing.Filter;

public abstract class DrawingControllerFilterBaseTests : DrawingControllerBaseTest
{
    protected readonly bool onlyIfVisible;

    protected DrawingControllerFilterBaseTests(bool onlyIfVisible)
    {
        this.onlyIfVisible = onlyIfVisible;
    }


    //protected async Task Base_Filter_Ok()
    //{
    //    var filter = Generate_Ok_Filter();
    //    var expectedDrawing = Generate_Ok_Drawings();

    //    var result = await MockDrawingFilter(filter, expectedDrawing);

    //    var response = result.Assert_OkObjectResult();

    //    Assert_Filter_Ok(expectedDrawing, response);
    //}

    //protected abstract IEnumerable<DrawingModel> Generate_Ok_Drawings();
    protected abstract DrawingFilter Generate_Ok_Filter();

    //protected abstract Task<ActionResult<FilterResults>> MockDrawingFilter(DrawingFilter filter, FilterResults expectedResults);
    //protected abstract void Assert_Filter_Ok(FilterResults expected, FilterResults response);


    public async Task Base_Filter_Error_InternalError()
    {
        var filter = Generate_Ok_Filter();
        var expectedError = DrawingFilterErrorMessages.InternalServer;

        var result = await MockDrawingFilterInternalError(filter);

        result
            .Assert_InternalErrorResult()
            .Assert_ErrorResponse(expectedError);
    }

    public async Task<ActionResult<FilterResults>> MockDrawingFilterInternalError(DrawingFilter filter)
    {
        _mockAppService
            .Setup(s => s.FilterDrawingsAsync(filter))
            .ThrowsAsync(new Exception());

        return await FetchFilter(filter);
    }

    protected abstract Task<ActionResult<FilterResults>> FetchFilter(DrawingFilter filter);
}
