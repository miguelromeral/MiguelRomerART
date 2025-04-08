using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MRA.DTO.Options;

public class AppConfiguration
{
    public JwtOptions Jwt { get; set; }
    public AdministratorOptions Administrator { get; set; }
    public AzureKeyVaultOptions AzureKeyVault { get; set; }
    public AzureStorageOptions AzureStorage { get; set; }
    public CacheOptions Cache { get; set; }
    public FirebaseOptions Firebase { get; set; }
    public MRALoggerOptions MRALogger { get; set; }
}
