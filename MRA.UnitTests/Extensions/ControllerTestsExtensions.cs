using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.UnitTests.Extensions;

public static class ControllerTestsExtensions
{
    public static OkObjectResult Assert_OkObjectResult<T>(this ActionResult<T> result)
    {
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsType<T>(okResult.Value);
        return okResult;
    }

    public static NotFoundObjectResult Assert_NotFoundResult<T>(this ActionResult<T> result)
    {
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        return notFoundResult;
    }

    public static ObjectResult Assert_InternalErrorResult<T>(this ActionResult<T> result)
    {
        Assert.NotNull(result);
        Assert.IsType<ObjectResult>(result.Result);

        var errorResult = result.Result as ObjectResult;
        Assert.NotNull(errorResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        return errorResult;
    }

    public static void Assert_NotFoundResponse(this ObjectResult result, string expectedError)
    {
        var errorResponse = result.Value as NotFoundResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(expectedError, errorResponse.Message);
    }
}
