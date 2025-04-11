
using MRA.Infrastructure.Configuration.Options;

namespace MRA.Infrastructure.Configuration;

public class AppConfiguration
{
    public JwtOptions Jwt { get; set; }
    public AdministratorOptions Administrator { get; set; }
    public AzureKeyVaultOptions AzureKeyVault { get; set; }
    public AzureStorageOptions AzureStorage { get; set; }
    public CacheOptions Cache { get; set; }
    public FirebaseOptions Firebase { get; set; }
    public MRALoggerOptions MRALogger { get; set; }
    public EPPlusOptions EPPlus { get; set; }
    public AzureCosmosDbOptions AzureCosmosDb { get; set; }
    public DatabaseOptions Database { get; set; }
}
