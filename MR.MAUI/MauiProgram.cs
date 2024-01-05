using Google.Api;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MRA.Services;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Interfaces;

namespace MR.MAUI
{
    public interface IServiceExample
    {
        void Hola();
    }

    public class ServiceExample : IServiceExample
    {
        public void Hola()
        {
            var tmp = "Hola";
        }
    }

    //public class MainViewModel
    //{
    //    private readonly IServiceExample _serviceExample;

    //    public MainViewModel(IServiceExample deviceService)
    //    {
    //        _serviceExample = deviceService;
    //    }
    //}

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            ConfigureServices(builder);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static async void ConfigureServices(MauiAppBuilder builder)
        {
            var appSettings = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = appSettings["AzureStorage:ConnectionString"];
            var blobStorageContainer = appSettings["AzureStorage:BlobStorageContainer"];
            var blobURL = appSettings["AzureStorage:BlobPath"];

            var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);

            builder.Services.AddSingleton(azureStorageService);

            var file = @"romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json";

            //new path we will copy the file to. 
            var newPath = Path.Combine(FileSystem.CacheDirectory, file);

            // read the file in Resource/Raw in this way
            using var json = await FileSystem.OpenAppPackageFileAsync(file);
            //create the new path and copy the original json file here
            using var dest = File.Create(newPath);
            await json.CopyToAsync(dest);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", newPath);
            dest.Close();

            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Resources\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

            // Configurar la conexión a Firestore
            var firestoreDbBuilder = new FirestoreDbBuilder { ProjectId = appSettings["Firebase:ProjectID"] };
            var db = firestoreDbBuilder.Build();

            //var db = FirestoreDb.Create();

            var firebaseService = new FirestoreService(
                        appSettings["Firebase:CollectionDrawings"],
                        appSettings["Firebase:CollectionInspirations"],
                        appSettings["Firebase:CollectionCollections"],
                        appSettings["AzureStorage:BlobPath"],

                db);

            builder.Services.AddSingleton<IFirestoreService>(firebaseService);

            var drawingService = new DrawingService(azureStorageService, firebaseService);

            builder.Services.AddSingleton(drawingService);

            builder.Services.AddSingleton<IServiceExample, ServiceExample>();

            //// Registra el servicio utilizando un método de fábrica
            //services.AddSingleton<IDra>(sp =>
            //{
            //    return drawingService;
            //});
        }
    }
}

