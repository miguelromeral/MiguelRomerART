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

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Logging.ClearProviders();  // Limpia los proveedores de logging preexistentes
builder.Logging.AddConsole();      // Agrega logging en consola (visible en Kudu)
builder.Logging.AddDebug();        // Agrega logging de depuración

// Si necesitas logs de mayor detalle en producción, ajusta el nivel de logging
builder.Logging.SetMinimumLevel(LogLevel.Information);  // Cambia a Debug o Trace si necesitas más detalle

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>(); // Obtener el logger
logger.LogInformation("Iniciando la aplicación...");

// Añadimos Autenticación por JWT
logger.LogInformation("Iniciando la configuración de JWT");
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

// Configuración de Azure
logger.LogInformation("Configurando conexión con Azure Storage");
var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
var blobStorageContainer = builder.Configuration.GetValue<string>("AzureStorage:BlobStorageContainer");
var blobURL = builder.Configuration.GetValue<string>("AzureStorage:BlobPath");

var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);
builder.Services.AddSingleton(azureStorageService);

// Configuración de Credenciales de Firebase
logger.LogInformation("Configurando credenciales de Google Firebase");
var serviceAccountPath = "";

// Si estás en Azure, crea el archivo temporal desde la variable de entorno
var googleCredentialsJson = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON");
if (!string.IsNullOrEmpty(googleCredentialsJson))
{
    logger.LogInformation("Detectada variable de entorno para las credenciales. Creando fichero temporal");
    var tempCredentialPath = Path.Combine(Path.GetTempPath(), "firebase-credentials.json");
    File.WriteAllText(tempCredentialPath, googleCredentialsJson);

    // Establecer la variable de entorno GOOGLE_APPLICATION_CREDENTIALS para que apunte al archivo temporal
    serviceAccountPath = tempCredentialPath;
}
else
{
    // Si estoy en local
    logger.LogInformation("Obteniendo credenciales desde fichero almacenado en el sistema de archivos");
    serviceAccountPath = @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json";
}
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

// Configuración de Firebase
logger.LogInformation("Configurando credenciales de Google Firebase");
var firebaseProjectId = builder.Configuration.GetValue<string>("Firebase:ProjectID");
var secondsCache = builder.Configuration.GetValue<int>("CacheSeconds");

//var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(serviceAccountPath);
logger.LogInformation("Creando servicio de Remote Config");
var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseProjectId, serviceAccountPath, secondsCache);
builder.Services.AddSingleton(remoteConfigService);

logger.LogInformation("Creando servicio de Firebase");
var firebaseService = new FirestoreService(firebaseProjectId, builder.Configuration.GetValue<string>("AzureStorage:BlobPath"));
firebaseService.SetCollectionNames(
    builder.Configuration.GetValue<string>("Firebase:CollectionDrawings"),
    builder.Configuration.GetValue<string>("Firebase:CollectionCollections"),
    builder.Configuration.GetValue<string>("Firebase:CollectionInspirations"));
firebaseService.SetRemoteConfigService(remoteConfigService);

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var drawingService = new DrawingService(secondsCache, new MemoryCache(new MemoryCacheOptions()), azureStorageService, firebaseService, remoteConfigService);

builder.Services.AddSingleton(drawingService);

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

logger.LogInformation("Creando aplicación");
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

logger.LogInformation("Aplicación creada con éxito. Procede a ejecutarse");

app.Run();