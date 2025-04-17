using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.UnitTests.Extensions;

public static class ControllerTestsExtensions
{
    public static T Assert_OkObjectResult<T>(this ActionResult<T> result)
    {
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsAssignableFrom<T>(okResult.Value);
        if (okResult.Value is T value)
        {
            return value;
        }
        Assert.Fail("Impossible to cast response object to the expected type");
        return default;
    }

    public static ObjectResult Assert_BadRequestResult<T>(this ActionResult<T> result)
    {
        return result.Assert_ServerResult<T, BadRequestObjectResult>(StatusCodes.Status400BadRequest);
    }

    public static ObjectResult Assert_NotFoundResult<T>(this ActionResult<T> result)
    {
        return result.Assert_ServerResult<T, NotFoundObjectResult>(StatusCodes.Status404NotFound);
    }

    public static ObjectResult Assert_InternalErrorResult<T>(this ActionResult<T> result)
    {
        return result.Assert_ServerResult<T, ObjectResult>(StatusCodes.Status500InternalServerError);
    }

    public static ObjectResult Assert_ServiceUnavailable<T>(this ActionResult<T> result)
    {
        return result.Assert_ServerResult<T, ObjectResult>(StatusCodes.Status503ServiceUnavailable);
    }

    private static ObjectResult Assert_ServerResult<TResponse, TResult>(this ActionResult<TResponse> result, int statusCode)
    {
        Assert.NotNull(result);
        Assert.IsNotType<OkObjectResult>(result.Result);
        Assert.IsType<TResult>(result.Result);

        var errorResult = result.Result as ObjectResult;
        Assert.NotNull(errorResult);
        Assert.Equal(statusCode, errorResult.StatusCode);
        return errorResult;
    }

    public static void Assert_ErrorResponse(this ObjectResult result, string expectedError)
    {
        var errorResponse = result.Value as ErrorResponse;
        Assert.NotNull(errorResponse);
        Assert.Equal(expectedError, errorResponse.Message);
    }
}
