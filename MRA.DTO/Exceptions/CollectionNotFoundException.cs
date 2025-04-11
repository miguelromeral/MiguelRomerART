
namespace MRA.DTO.Exceptions;

public class CollectionNotFoundException : DocumentNotFoundException
{
    public CollectionNotFoundException() : base("")
    {
    }

    public CollectionNotFoundException(string drawingId)
        : base(CustomMessage(drawingId))
    {
    }

    public CollectionNotFoundException(string drawingId, Exception inner)
        : base(CustomMessage(drawingId), inner)
    {
    }

    public static string CustomMessage(string id) => $"The collection with ID \"{id}\" was not found.";
}

