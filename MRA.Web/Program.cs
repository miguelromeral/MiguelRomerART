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
using Google.Api;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
var blobStorageContainer = builder.Configuration.GetValue<string>("AzureStorage:BlobStorageContainer");
var blobURL = builder.Configuration.GetValue<string>("AzureStorage:BlobPath");

var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);
builder.Services.AddSingleton(azureStorageService);

var firebaseService = new FirestoreService(builder.Configuration, builder.Configuration.GetValue<string>("AzureStorage:BlobPath"));

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var drawingService = new DrawingService(builder.Configuration.GetValue<int>("CacheSeconds"), new MemoryCache(new MemoryCacheOptions()), azureStorageService, firebaseService, null);

builder.Services.AddSingleton(drawingService);

builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "CookieMiguelRomeral";
        options.LoginPath = "/Admin/Login";
    });

builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

var firestoreService = app.Services.GetRequiredService<IFirestoreService>();
Console.WriteLine($"Firestore Service created: {firestoreService}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
