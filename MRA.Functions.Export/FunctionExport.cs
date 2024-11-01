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
        public async Task Run(
#if DEBUG
        [TimerTrigger("0 */1 * * * *")]
#else
        [TimerTrigger("30 12 * * *")] // Every day at 12:30 UTC
#endif
         TimerInfo myTimer)
        {
            var console = new ConsoleHelper();
            MRLogger logger = null;
            try
            {
                logger = new MRLogger(_configuration, console);
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

                logger.Log("Leyendo documentos desde Firestore");
                List<Drawing> listDrawings;

#if DEBUG
                listDrawings = new List<Drawing>(){
                    await firestoreService.FindDrawingByIdAsync("cloud")
                };
#else
                listDrawings = await firestoreService.GetDrawingsAsync();
#endif
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

                    logger.Log("Preparando fichero para guardar en Azure Storage");
                    // Crear un MemoryStream para guardar el archivo en memoria
                    using (var memoryStream = new MemoryStream())
                    {
                        var fileName = excelService.GetFileName();
                        // Guardar el contenido del ExcelPackage en el MemoryStream
                        excel.SaveAs(memoryStream);

                        // Restablecer la posición del Stream al inicio
                        memoryStream.Position = 0;

                        try
                        {
                            // Llamar a la función para subir a Azure, pasando el MemoryStream
                            await azureStorageService.GuardarExcelEnAzureStorage(memoryStream, azureStorageService.ExportLocation, fileName);
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"Error al guardar el archivo \"{fileName}\" en Azure Storage: " + ex.Message);
                        }
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
