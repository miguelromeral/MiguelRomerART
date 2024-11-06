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
using MRA.DTO.Logger;
using MRA.Services.AzureStorage;

var console = new ConsoleHelper();
MRLogger logger = null;
try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    logger = new MRLogger(configuration, console);
    logger.LogInformation("Iniciando Aplicación de Exportación");

    var automated = configuration.GetValue<bool>("Commands:Automated");
    logger.LogInformation($"Automatizado: {(automated ? "SÍ" : "NO")}");

    var excelService = new ExcelService(configuration, logger);

    logger.Log("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

    logger.Log("Configurando Azure Service");
    var azureStorageService = new AzureStorageService(configuration);

    logger.Log("Registrando credenciales de Firebase");
    var firestoreService = new FirestoreService(configuration);

    var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    firestoreService.SetRemoteConfigService(remoteConfigService);

    if (automated)
    {
        logger.LogInformation($"Ejecución AUTOMATIZADA en entorno de {(firestoreService.IsInProduction ? "PRODUCCIÓN" : "PRE")}");
    }
    else
    {
        if (!logger.ProductionEnvironmentAlert(firestoreService.IsInProduction))
        {
            return;
        }
    }

    logger.Log("Leyendo documentos desde Firestore");
    var listDrawings = await firestoreService.GetDrawingsAsync();
    listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

    // Crear un nuevo archivo Excel
    using (ExcelPackage excel = new ExcelPackage())
    {
        logger.Log($"Creando hoja principal \"{ExcelService.EXCEL_DRAWING_SHEET_NAME}\"");
        var workSheet = excel.Workbook.Worksheets.Add(ExcelService.EXCEL_DRAWING_SHEET_NAME);
        workSheet.View.FreezePanes(2, 2);

        logger.Log("Obteniendo propiedades del DTO de Drawing");
        var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();

        logger.Log("Rellenando tabla principal");
        excelService.FillDrawingTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

        logger.Log("Preparando hojas de diccionarios");
        excelService.FillSheetsDictionary(excel, drawingProperties, workSheet);

        logger.Log("Preparando fichero para guardar en sistema de archivos");
        var fileInfo = excelService.GetFileInfo();

        if (excelService.SaveFileLocally)
        {
            excel.SaveAs(fileInfo);
        }

        logger.Log("Preparando fichero para guardar en Azure Storage");
        await azureStorageService.GuardarExcelEnAzureStorage(fileInfo, azureStorageService.ExportLocation);

        logger.Success($"Archivo Excel creado: \"{fileInfo.FullName}\"");
    }
}
catch (Exception ex)
{
    logger?.LogError("Error durante la exportación: " + ex.Message);
    console.ShowMessageInfo("Pulse cualquier tecla para continuar");
    Console.ReadKey();
}