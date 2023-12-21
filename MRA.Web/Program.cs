using Google.Cloud.Firestore;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using Microsoft.Extensions.Configuration;
using MRA.Services.AzureStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var azureStorageService = new AzureStorageService(builder.Configuration);

builder.Services.AddSingleton(azureStorageService);

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Properties\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

//builder.Services.AddSingleton<IFirestoreService>(s => 
//    new FirestoreService(FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID"))));

var firebaseService = new FirestoreService(builder.Configuration, FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID")));

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var drawingService = new DrawingService(azureStorageService, firebaseService);

builder.Services.AddSingleton(drawingService);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

var firestoreService = app.Services.GetRequiredService<IFirestoreService>();
Console.WriteLine($"Firestore Service created: {firestoreService}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
