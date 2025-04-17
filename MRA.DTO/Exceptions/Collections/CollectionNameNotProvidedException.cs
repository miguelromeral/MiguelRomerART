namespace MRA.DTO.Exceptions.Collections;

public class CollectionNameNotProvidedException : DocumentNotFoundException
{
    public static string ErrorMessage(string name) => $"The collection with name \"{name}\" was not provided.";

    public CollectionNameNotProvidedException(string collectionName)
        : base(ErrorMessage(collectionName))
    {
    }
}
