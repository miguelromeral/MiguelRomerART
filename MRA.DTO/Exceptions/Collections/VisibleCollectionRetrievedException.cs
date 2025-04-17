namespace MRA.DTO.Exceptions.Collections;

public class VisibleCollectionRetrievedException : Exception
{
    public const string ErrorMessage = "Visible collections were found. Service unavailable";

    public VisibleCollectionRetrievedException()
        : base(ErrorMessage)
    {

    }
}
