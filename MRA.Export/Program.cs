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

    var excelService = new ExcelService(configuration, console);

    // Configuración de EPPlus
    console.ShowMessageInfo("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

    // Configuración de Firestore
    console.ShowMessageInfo("Registrando credenciales de Firebase");

    var firestoreService = new FirestoreService(configuration);

    var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    firestoreService.SetRemoteConfigService(remoteConfigService);

    console.ShowMessageInfo("Recuperando documentos desde Firestore");
    var listDrawings = await firestoreService.GetDrawingsAsync();
    listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

    // Crear un nuevo archivo Excel
    using (ExcelPackage excel = new ExcelPackage())
    {
        var workSheet = excel.Workbook.Worksheets.Add(excelService.SheetName);
        workSheet.View.FreezePanes(2, 2);

        var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();

        excelService.SetTableHeaders(ref workSheet, drawingProperties);
        excelService.FillTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

        console.ShowMessageInfo("Preparando formato de Tabla");
        ExcelService.CreateTable(ref workSheet, excelService.TableName, 1, 1, listDrawings.Count + 1, drawingProperties.Count);
        ExcelService.SetBold(ref workSheet, 2, 1, listDrawings.Count + 1, 1);
        
        console.ShowMessageInfo("Preparando hojas de diccionarios");
        ExcelService.CreateWorksheetDictionary(
            excel,
            name: "Styles", Drawing.DRAWING_TYPES, drawingProperties, workSheet,
            nameColumnDropdown: "Type",
            nameColumnIndex: "#Type");
        ExcelService.CreateWorksheetDictionary(
            excel,
            name: "Products", Drawing.DRAWING_PRODUCT_TYPES, drawingProperties, workSheet,
            nameColumnDropdown: "Product Type",
            nameColumnIndex: "#Product Type");
        ExcelService.CreateWorksheetDictionary(
            excel,
            name: "Software", Drawing.DRAWING_SOFTWARE, drawingProperties, workSheet,
            nameColumnDropdown: "Software",
            nameColumnIndex: "#Software");
        ExcelService.CreateWorksheetDictionary(
            excel,
            name: "Papers", Drawing.DRAWING_PAPER_SIZE, drawingProperties, workSheet,
            nameColumnDropdown: "Paper",
            nameColumnIndex: "#Paper");
        ExcelService.CreateWorksheetDictionary(
            excel,
            name: "Filters", Drawing.DRAWING_FILTER, drawingProperties, workSheet,
            nameColumnDropdown: "Filter",
            nameColumnIndex: "#Filter");

        console.ShowMessageInfo("Preparando fichero para guardar");
        var fileInfo = excelService.GetFileInfo();
        excel.SaveAs(fileInfo);

        console.ShowMessageInfo("Archivo Excel creado: " + fileInfo.FullName);
    }
}
catch (Exception ex)
{
    console.ShowMessageError("Ha ocurrido un error: " + ex.Message);
    console.ShowMessageInfo("Pulse cualquier tecla para continuar");
    Console.ReadKey();
}