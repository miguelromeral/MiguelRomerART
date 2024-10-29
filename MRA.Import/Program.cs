using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using MRA.Services.Helpers;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using MRA.Services.Firebase;
using MRA.DTO.Excel.Attributes;
using MRA.DTO.Firebase.Models;
using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Excel;

var console = new ConsoleHelper();
console.ShowMessageInfo("Cargando la configuración de la aplicación");
try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    var excelService = new ExcelService(configuration);

    // Configuración de EPPlus
    console.ShowMessageInfo("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

    // Configuración de Firestore
    console.ShowMessageInfo("Registrando credenciales de Firebase");

    var firestoreService = new FirestoreService(configuration);

    //var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    //firestoreService.SetRemoteConfigService(remoteConfigService);

    //helper.ShowMessageInfo("Recuperando documentos desde Firestore");
    //var listDrawings = await firestoreService.GetDrawingsAsync();
    //listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

    //var filePath = console.FillStringValue("File to Read");
    var filePath = "M:\\Descargas\\Excels_MiguelRomerART\\FirestoreDrawings_20241029_1755.xlsx";

    var fileInfo = new FileInfo(filePath);
    var listDrawings = excelService.ImportDrawingsFromExcel(fileInfo);



    console.ShowMessageInfo($"List of Drawings: {listDrawings.Count}");
}
catch (Exception ex)
{
    console.ShowMessageError("Ha ocurrido un error: " + ex.Message);
}
    console.ShowMessageInfo("Pulse cualquier tecla para continuar");
    Console.ReadKey();