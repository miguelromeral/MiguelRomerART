using Microsoft.AspNetCore.Authentication.Cookies;
using MRA.WebApi.Startup;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddCustomLogging(builder.Configuration, builder.Environment);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddAppSettingsFiles(builder.Environment.EnvironmentName)
    .AddEnvironmentVariables();

if (builder.Environment.IsProduction())
    builder.Configuration.ConfigureKeyVault(builder.Configuration);

builder.Services.AddDependencyInjectionServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCorsPolicies();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "CookieMiguelRomeral";
        options.LoginPath = "/Admin/Login";
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
