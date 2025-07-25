namespace MRA.WebApi.Models.Requests.Azure;

public class UploadAzureBlobRequest
{
    public IFormFile File { get; set; }
    public string Path { get; set; }
}
