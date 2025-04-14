
namespace MRA.Infrastructure.Settings.Options;

public class AzureStorageSettings
{
    public string ConnectionString { get; set; }
    public string BlobStorageContainer { get; set; }
    public string BlobPath { get; set; }
    public string ExportLocation { get; set; }
}
