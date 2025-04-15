using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MRA.Infrastructure.Settings.Options;
using System.Text;

namespace MRA.WebApi.Startup;

public static class AuthenticationStartup
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSection);
        
        var jwtOptions = jwtSection.Get<JwtSettings>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Key))
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine("Token validation failed.");
                    return Task.CompletedTask;
                }
            };
        });

        //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .AddCookie("Cookies", options =>
        //    {
        //        options.Cookie.Name = "CookieMiguelRomeral";
        //        options.LoginPath = "/Admin/Login";
        //    });

        services.AddAuthorization();
    }
}
