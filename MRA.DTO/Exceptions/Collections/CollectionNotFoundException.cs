namespace MRA.DTO.Exceptions.Collections;

public class CollectionNotFoundException : DocumentNotFoundException
{
    public static string ErrorMessage(string collectionId) =>  $"The collection with ID '{collectionId}' was not found.";

    public CollectionNotFoundException(string collectionId)
    : base(ErrorMessage(collectionId))
    {
    }
}