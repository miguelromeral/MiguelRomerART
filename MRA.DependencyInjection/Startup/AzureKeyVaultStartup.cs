using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace MRA.DependencyInjection.Startup;
public static class AzureKeyVaultStartup
{
    public static void ConfigureKeyVault(this IConfigurationBuilder configuration, IConfiguration config)
    {
        var keyVaultURL = config.GetValue<string>("AzureKeyVault:URL");
        var credential = new DefaultAzureCredential();
        configuration.AddAzureKeyVault(new Uri(keyVaultURL), credential);
    }
}
