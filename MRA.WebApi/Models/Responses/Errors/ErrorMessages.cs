namespace MRA.WebApi.Models.Responses.Errors;

public static class ErrorMessages
{       
    public static class CollectionErrorMessages
    {
        public static class FetchDetails
        {
            public static string InternalServer(string id) => $"Error when retrieving collection with ID '{id}'";

        }

        public static class FetchList
        {
            public const string InternalServer = "Error when fetching collections.";
        }


        public static class Save
        {
            public const string IdNotProvided = "No correct ID provided for the collection";

            public static string InternalServer(string id) => $"Error when saving collection '{id}'";
        }

        public static class Delete
        {
            public static string InternalServer(string id) => $"Error when deletting collection '{id}'";
        }
    }
}
