namespace MRA.WebApi.Models.Responses
{
    public class CheckAzurePathResponse
    {
        public bool Existe { get; set; }
        public string Url { get; set; }
        public string UrlThumbnail { get; set; }
        public string PathThumbnail { get; set; }
    }
}
