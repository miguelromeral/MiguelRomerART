//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using MRA.DTO.Options;
//using System.Text;

//namespace MRA.DependencyInjection.Startup;

//public static class AuthenticationStartup
//{
//    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
//    {
//        var jwtSection = configuration.GetSection("Jwt");
//        services.Configure<JwtOptions>(jwtSection);
        
//        var jwtOptions = jwtSection.Get<JwtOptions>();

//        services.AddAuthentication(options =>
//        {
//            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        })
//        .AddJwtBearer(options =>
//        {
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = jwtOptions.Issuer,
//                ValidAudience = jwtOptions.Audience,
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Key))
//            };
//        });
//    }
//}
