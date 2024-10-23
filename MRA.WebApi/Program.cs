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

var builder = WebApplication.CreateBuilder(args);

// Agregar JWT

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
        builder.WithOrigins("http://localhost:4200", "https://solomanitas.azurewebsites.net")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
var blobStorageContainer = builder.Configuration.GetValue<string>("AzureStorage:BlobStorageContainer");
var blobURL = builder.Configuration.GetValue<string>("AzureStorage:BlobPath");

var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);

builder.Services.AddSingleton(azureStorageService);

var serviceAccountPath = @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json";

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

var firebaseProjectId = builder.Configuration.GetValue<string>("Firebase:ProjectID");
var secondsCache = builder.Configuration.GetValue<int>("CacheSeconds");

//var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(serviceAccountPath);
var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firebaseProjectId, serviceAccountPath, secondsCache);
builder.Services.AddSingleton(remoteConfigService);

var firebaseService = new FirestoreService(
            builder.Configuration.GetValue<string>("Firebase:CollectionDrawings"),
            builder.Configuration.GetValue<string>("Firebase:CollectionInspirations"),
            builder.Configuration.GetValue<string>("Firebase:CollectionCollections"),
            builder.Configuration.GetValue<string>("Firebase:CollectionExperience"),
            builder.Configuration.GetValue<string>("AzureStorage:BlobPath"),
    FirestoreDb.Create(firebaseProjectId),
    remoteConfigService);
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

builder.Services.AddMemoryCache();

var app = builder.Build();

// JWT

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

var firestoreService = app.Services.GetRequiredService<IFirestoreService>();
Console.WriteLine($"Firestore Service created: {firestoreService}");

app.MapControllers();

app.Run();
