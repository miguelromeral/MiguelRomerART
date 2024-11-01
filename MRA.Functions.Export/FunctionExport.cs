using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Logger;
using MRA.Services.AzureStorage;
using MRA.Services.Excel;
using MRA.Services.Firebase;
using MRA.Services.Helpers;
using OfficeOpenXml;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        //private readonly ILogger<FunctionExport> _logger;
        private readonly IConfiguration _configuration;

        public FunctionExport(
            //ILogger<FunctionExport> logger, 
            IConfiguration configuration)
        {
            //_logger = logger;
            _configuration = configuration;
        }


        [FunctionName("FunctionExport")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            var console = new ConsoleHelper();
            Logger logger = null;
            try
            {
                logger = new Logger(_configuration, console);
                logger.Info("Iniciando Aplicación de Exportación");

                var excelService = new ExcelService(_configuration, logger);

                logger.Log("Configurando EPPlus");
                ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

                logger.Log("Configurando Azure Service");
                var azureStorageService = new AzureStorageService(_configuration);

                logger.Log("Registrando credenciales de Firebase");
                var firestoreService = new FirestoreService(_configuration);

                //var remoteConfigService = new RemoteConfigService(null, firestoreService.ProjectId, firestoreService.CredentialsPath, 1);
                //firestoreService.SetRemoteConfigService(remoteConfigService);

                logger.Info($"Ejecución AUTOMATIZADA en entorno de {(firestoreService.IsInProduction ? "PRODUCCIÓN" : "PRE")}");

                //logger.Log("Leyendo documentos desde Firestore");
                ////var listDrawings = await firestoreService.GetDrawingsAsync();
                var listDrawings = new List<Drawing>(){
                    await firestoreService.FindDrawingByIdAsync("cloud")
                };
                listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

                logger.Warning($"Popularidad de Cloud: {listDrawings[0].Popularity}");

                // Crear un nuevo archivo Excel
                using (ExcelPackage excel = new ExcelPackage())
                {
                    logger.Log($"Creando hoja principal \"{excelService.SheetName}\"");
                    var workSheet = excel.Workbook.Worksheets.Add(excelService.SheetName);
                    workSheet.View.FreezePanes(2, 2);

                    logger.Log("Obteniendo propiedades del DTO de Drawing");
                    var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();

                    excelService.SetTableHeaders(ref workSheet, drawingProperties);

                    logger.Log("Rellenando tabla principal");
                    excelService.FillTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

                    logger.Log("Preparando hojas de diccionarios");
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

                    logger.Log("Preparando fichero para guardar en sistema de archivos");
                    var fileInfo = excelService.GetFileInfo();
                    excel.SaveAs(fileInfo);
                    logger.Success($"Archivo Excel creado: \"{fileInfo.FullName}\"");

                    logger.Log("Preparando fichero para guardar en Azure Storage");
                    try
                    {
                        await azureStorageService.GuardarExcelEnAzureStorage(fileInfo, azureStorageService.ExportLocation);
                    }catch(Exception ex)
                    {
                        logger.Error("Error al guardar el archivo en Azure Storage: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Error durante la exportación: " + ex.Message);
            }
            logger.Log("Fin de la Exportación en Azure Functions");
        }
    }
}
