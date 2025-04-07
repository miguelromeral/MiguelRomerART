using Google.Cloud.Firestore;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using Microsoft.Extensions.Configuration;
using MRA.Services.AzureStorage;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Firestore.V1;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Runtime.Intrinsics.Arm;
using MRA.Services.Helpers;
using MRA.DTO.Logger;
using MRA.WebApi.Controllers;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Logger;
using Azure.Identity;
using MRA.DTO.Options;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using FirebaseAdmin.Auth.Multitenancy;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Logging.ClearProviders();  // Limpia los proveedores de logging preexistentes
builder.Logging.AddConsole();      // Agrega logging en consola (visible en Kudu)
builder.Logging.AddDebug();        // Agrega logging de depuración
//builder.Logging.SetMinimumLevel(LogLevel.Information);  // Cambia a Debug o Trace si necesitas más detalle
builder.Logging.AddProvider(new MRLoggerProvider(builder.Configuration));

// Añadimos Autenticación por JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Agrega servicios CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://miguelromeral.azurewebsites.net/")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile($"appsettings.Local.json", optional: true)
    .AddEnvironmentVariables();

var logger = new MRLogger(builder.Configuration);

if (builder.Environment.IsProduction())
{
    logger.LogInformation("Configurando Azure Key Vault");
    var keyVaultURL = builder.Configuration.GetValue<string>("AzureKeyVault:URL");
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultURL), new DefaultAzureCredential());
}

builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureStorage"));

logger.LogInformation("Configurando Azure Storage Service");
builder.Services.AddSingleton<AzureStorageService>();

// Configuración de Firebase
logger.LogInformation("Configurando Firebase");
var secondsCache = builder.Configuration.GetValue<int>("CacheSeconds");

var firebaseService = new FirestoreService(builder.Configuration, new FirestoreDatabase(), logger);

//var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(serviceAccountPath);
var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseService.ProjectId, firebaseService.CredentialsPath, secondsCache);
builder.Services.AddSingleton(remoteConfigService);

firebaseService.SetRemoteConfigService(remoteConfigService);

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var azureStorageService = builder.Services.BuildServiceProvider().GetRequiredService<AzureStorageService>();
var drawingService = new DrawingService(secondsCache, new MemoryCache(new MemoryCacheOptions()), azureStorageService, firebaseService, remoteConfigService, logger);

builder.Services.AddSingleton<IDrawingService>(drawingService);

// Configura el servicio de caché distribuido en memoria
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "CookieMiguelRomeral";
        options.LoginPath = "/Admin/Login";
    });

// Añadir Memoria Cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Utilizando autenticación JWT
app.UseAuthentication();
app.UseAuthorization();


//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Usa el middleware de CORS antes de autenticación y autorización
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

logger.LogInformation("Aplicación configurada correctamente. Iniciando.");

app.Run();