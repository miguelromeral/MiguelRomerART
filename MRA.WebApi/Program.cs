using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using MRA.Services.AzureStorage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Cookies;
using MRA.DTO.Logger;
using MRA.WebApi.Startup;
using MRA.DependencyInjection;
using MRA.DTO.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettings();

if (builder.Environment.IsProduction())
    builder.Configuration.ConfigureKeyVault(builder.Configuration);

//builder.Services.AddCustomConfiguration(builder.Configuration);
builder.Services.AddDependencyInjectionServices(builder.Configuration);

builder.Logging.AddCustomLogging(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCorsPolicies();

builder.Services.AddAzureStorage(builder.Configuration);

var logger = new MRLogger(builder.Configuration);
var appConfig = builder.Services.BuildServiceProvider().GetRequiredService<AppConfiguration>();

builder.Services.AddFirebase(builder.Configuration, logger, appConfig);

var azureStorageService = builder.Services.BuildServiceProvider().GetRequiredService<AzureStorageService>();
var drawingService = new DrawingService(new MemoryCache(new MemoryCacheOptions()),
    azureStorageService,
    builder.Services.BuildServiceProvider().GetRequiredService<IFirestoreService>(),
    builder.Services.BuildServiceProvider().GetRequiredService<RemoteConfigService>(),
    logger,
    appConfig);

builder.Services.AddSingleton<IDrawingService>(drawingService);

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

logger.LogInformation("Aplicación configurada correctamente. Iniciando.");
app.Run();
