
using MRA.Infrastructure.Settings.Options;
using MRA.Infrastructure.Settings.Sections;

namespace MRA.Infrastructure.Settings;

public class AppSettings
{
    public JwtSettings Jwt { get; set; }
    public AdministratorSettings Administrator { get; set; }
    public AzureKeyVaultSettings AzureKeyVault { get; set; }
    public AzureStorageSettings AzureStorage { get; set; }
    public CacheSettings Cache { get; set; }
    public FirebaseSettings Firebase { get; set; }
    public MRALoggerSettings MRALogger { get; set; }
    public EPPlusSettings EPPlus { get; set; }
    public AzureCosmosSettings AzureCosmosDb { get; set; }
    public DatabaseSettings Database { get; set; }
    public RemoteConfigSettings RemoteConfig { get; set; }
    public CommandsSettings Commands { get; set; }

    public AppSettings()
    {

    }
}
