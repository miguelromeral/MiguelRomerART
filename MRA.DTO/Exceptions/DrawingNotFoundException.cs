
namespace MRA.DTO.Exceptions;

public class DrawingNotFoundException : DocumentNotFoundException
{
    public DrawingNotFoundException() : base("")
    {
    }

    public DrawingNotFoundException(string drawingId)
        : base(CustomMessage(drawingId))
    {
    }

    public DrawingNotFoundException(string drawingId, Exception inner)
        : base(CustomMessage(drawingId), inner)
    {
    }

    public static string CustomMessage(string id) => $"The drawing with ID \"{id}\" was not provided.";
}
