namespace MRA.WebApi.Startup;

public static class CORSPoliciesStartup
{
    public static void AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins("http://localhost:4200", "https://miguelromeral.azurewebsites.net/")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });
    }
}
