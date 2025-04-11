namespace MRA.Infrastructure.Configuration.Options;

public class AzureCosmosDbOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public int TimeoutSeconds { get; set; }
}
