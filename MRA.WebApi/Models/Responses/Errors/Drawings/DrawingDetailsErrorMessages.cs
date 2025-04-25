namespace MRA.WebApi.Models.Responses.Errors.Drawings;

public static class DrawingDetailsErrorMessages
{
    public static string NotFound(string id) => $"Drawing with ID '{id}' not found.";
    
    public static string InternalServer(string id) => $"Error when fetching drawing '{id}' details.";
}
