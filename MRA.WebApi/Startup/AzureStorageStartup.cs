using Azure.Identity;
using MRA.DTO.Options;
using MRA.Services.AzureStorage;

namespace MRA.WebApi.Startup;

public static class AzureStorageStartup
{
    public static void ConfigureKeyVault(this IConfigurationBuilder configuration, IConfiguration config)
    {
        var keyVaultURL = config.GetValue<string>("AzureKeyVault:URL");
        var credential = new DefaultAzureCredential();
        configuration.AddAzureKeyVault(new Uri(keyVaultURL), credential);
    }
}
