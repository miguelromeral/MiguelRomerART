using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MRA.DTO.Options;
using System.Text;

namespace MRA.WebApi.Startup;

public static class ConfigurationStartup
{
    public static void AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppConfiguration>(options =>
        {
            options.Jwt = configuration.GetSection("Jwt").Get<JwtOptions>();
            options.Administrator = configuration.GetSection("Administrator").Get<AdministratorOptions>();
        });
    }
}
