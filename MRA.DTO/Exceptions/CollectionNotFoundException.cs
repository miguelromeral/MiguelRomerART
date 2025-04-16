
namespace MRA.DTO.Exceptions;

public class CollectionNotFoundException : DocumentNotFoundException
{
    public CollectionNotFoundException() : base("")
    {
    }

    public CollectionNotFoundException(string id)
        : base(CustomMessage(id))
    {
    }

    public CollectionNotFoundException(string id, Exception inner)
        : base(CustomMessage(id), inner)
    {
    }

    public static string CustomMessage(string id) => $"The collection with ID \"{id}\" was not found.";
}

