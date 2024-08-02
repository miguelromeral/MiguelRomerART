namespace MRA.WebApi.Models.Responses
{
    public class UploadAzureImageResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Url { get; set; }
        public string UrlThumbnail { get; set; }
        public string PathThumbnail { get; set; }
    }
}
