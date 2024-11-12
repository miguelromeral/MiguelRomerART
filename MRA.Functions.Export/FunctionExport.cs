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
using MRA.Services.AzureStorage;
using MRA.Services.Excel;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Helpers;
using OfficeOpenXml;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        private readonly ILogger<FunctionExport> _logger;
        private readonly IConfiguration _configuration;

        public FunctionExport(
            ILogger<FunctionExport> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [FunctionName("FunctionExport")]
        public async Task Run(
#if DEBUG
        [TimerTrigger("0 */1 * * * *")]
#else
        [TimerTrigger("0 30 12 */1 * *")] // Every day at 12:30 UTC
#endif
         TimerInfo myTimer/*, ILogger log*/)
        {
            try
            {
                _logger.LogInformation("Iniciando Aplicación de Exportación");

                var excelService = new ExcelService(_configuration, _logger);

                _logger.LogInformation("Configurando EPPlus");
                ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

                _logger.LogInformation("Configurando Azure Service");
                var azureStorageService = new AzureStorageService(_configuration);

                _logger.LogInformation("Registrando credenciales de Firebase");
                var firestoreService = new FirestoreService(_configuration, new FirestoreDatabase());

                var remoteConfigService = new RemoteConfigService(null, firestoreService.ProjectId, firestoreService.CredentialsPath, 3600);
                firestoreService.SetRemoteConfigService(remoteConfigService);

                _logger.LogInformation($"Ejecución AUTOMATIZADA en entorno de {(firestoreService.IsInProduction ? "PRODUCCIÓN" : "PRE")}");

                _logger.LogInformation("Leyendo documentos desde Firestore");
                List<Drawing> listDrawings;

#if DEBUG
                listDrawings = new List<Drawing>(){
                    await firestoreService.FindDrawingByIdAsync("cloud")
                };
#else
                listDrawings = await firestoreService.GetDrawingsAsync();
#endif

                _logger.LogInformation("Calculando Popularidad");
                listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);


                // Crear un nuevo archivo Excel
                using (ExcelPackage excel = new ExcelPackage())
                {
                    _logger.LogInformation($"Creando hoja principal \"{ExcelService.EXCEL_DRAWING_SHEET_NAME}\"");
                    var workSheet = excel.Workbook.Worksheets.Add(ExcelService.EXCEL_DRAWING_SHEET_NAME);
                    workSheet.View.FreezePanes(2, 2);

                    _logger.LogInformation("Obteniendo propiedades del DTO de Drawing");
                    var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();

                    _logger.LogInformation("Rellenando tabla principal");
                    excelService.FillDrawingTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

                    _logger.LogInformation("Preparando hojas de diccionarios");
                    excelService.FillSheetsDictionary(excel, drawingProperties, workSheet);

                    _logger.LogInformation("Preparando fichero para guardar en Azure Storage");
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
                            _logger.LogError(ex, $"Error al guardar el archivo \"{fileName}\" en Azure Storage");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error durante la exportación");
            }
            _logger.LogInformation("Fin de la Exportación en Azure Functions");
        }
    }
}
