//using Azure.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using MRA.DTO.Options;
//using MRA.Services.AzureStorage;

//namespace MRA.DependencyInjection.Startup;

//public static class AzureStorageStartup
//{
//    public static void ConfigureKeyVault(this IConfigurationBuilder configuration, IConfiguration config)
//    {
//        var keyVaultURL = config.GetValue<string>("AzureKeyVault:URL");
//        var credential = new DefaultAzureCredential();
//        configuration.AddAzureKeyVault(new Uri(keyVaultURL), credential);
//    }

//    public static void AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.Configure<AzureStorageOptions>(configuration.GetSection("AzureStorage"));
//        services.AddSingleton<AzureStorageService>();
//    }
//}
