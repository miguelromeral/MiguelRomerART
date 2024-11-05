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
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para que lea la configuraci�n desde appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando la aplicaci�n...");

// A�adimos Autenticaci�n por JWT
logger.LogInformation("Iniciando la configuraci�n de JWT");
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
logger.LogInformation("Agregando servicios CORS");
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

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configuraci�n de Azure
logger.LogInformation("Configurando conexi�n con Azure Storage");

var azureStorageService = new AzureStorageService(builder.Configuration);
builder.Services.AddSingleton(azureStorageService);

// Configuraci�n de Firebase
logger.LogInformation("Configurando credenciales de Google Firebase");
var secondsCache = builder.Configuration.GetValue<int>("CacheSeconds");

logger.LogInformation("Creando servicio de Firebase");
var firebaseService = new FirestoreService(builder.Configuration, logger);

//var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(serviceAccountPath);
logger.LogInformation("Creando servicio de Remote Config");
var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseService.ProjectId, firebaseService.CredentialsPath, secondsCache);
builder.Services.AddSingleton(remoteConfigService);

firebaseService.SetRemoteConfigService(remoteConfigService);

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var drawingService = new DrawingService(secondsCache, new MemoryCache(new MemoryCacheOptions()), azureStorageService, firebaseService, remoteConfigService);

builder.Services.AddSingleton(drawingService);

// Configura el servicio de cach� distribuido en memoria
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

// A�adir Memoria Cache
builder.Services.AddMemoryCache();

logger.LogInformation("Creando aplicaci�n");
var app = builder.Build();

// Utilizando autenticaci�n JWT
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

// Usa el middleware de CORS antes de autenticaci�n y autorizaci�n
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

logger.LogInformation("Aplicaci�n creada con �xito. Procede a ejecutarse");

app.Run();