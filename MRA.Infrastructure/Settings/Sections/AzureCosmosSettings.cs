namespace MRA.Infrastructure.Settings.Options;

public class AzureCosmosSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public int TimeoutSeconds { get; set; }
}
