using Google.Cloud.Firestore;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton(new AzureStorageService(builder.Configuration));

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Properties\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

//builder.Services.AddSingleton<IFirestoreService>(s => 
//    new FirestoreService(FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID"))));

builder.Services.AddSingleton<IFirestoreService>(s =>
{
    try
    {
        return new FirestoreService(FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID")));
    }
    catch (Exception ex)
    {
        // Registra la excepción para análisis
        Console.WriteLine($"Error al crear FirestoreService: {ex}");
        throw; // Lanza la excepción nuevamente para interrumpir la aplicación
    }
});


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
