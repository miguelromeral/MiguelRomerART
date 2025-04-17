namespace MRA.WebApi.Models.Responses.Errors;

public class ErrorResponse
{
    public string Message { get; private set; }

    public ErrorResponse(string message)
    {
        Message = message;
    }
}
