
namespace MRA.Infrastructure.Configuration.Options;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; }
    public string BlobStorageContainer { get; set; }
    public string BlobPath { get; set; }
    public string ExportLocation { get; set; }
}
