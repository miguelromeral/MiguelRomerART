namespace MRA.WebApi.Models.Responses.Errors;

public class NotFoundResponse
{
    public string Message { get; private set; }

    public NotFoundResponse(string message)
    {
        Message = message;
    }
}
