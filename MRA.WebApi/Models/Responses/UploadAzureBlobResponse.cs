namespace MRA.WebApi.Models.Responses
{
    public class UploadAzureBlobResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
    }
}
