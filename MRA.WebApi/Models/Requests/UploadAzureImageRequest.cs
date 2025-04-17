namespace MRA.WebApi.Models.Requests
{
    public class UploadAzureImageRequest
    {
        public int Size { get; set; }
        public string Path { get; set; }
        public IFormFile Image { get; set; }
    }
}
