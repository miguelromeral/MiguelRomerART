using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Configuration;
using MRA.DTO.Firebase.Models;
using MRA.Services;
using MRA.Services.AzureStorage;
using MRA.Services.Excel;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.RemoteConfig;
using OfficeOpenXml;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        private readonly ILogger<FunctionExport> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IFirestoreService _firestoreService;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IAppService _drawingService;
        private readonly AppConfiguration _appConfiguration;

        public FunctionExport(
            ILogger<FunctionExport> logger,
            IConfiguration configuration,
            IAzureStorageService azureStorageService,
            IFirestoreService firestoreService,
            IRemoteConfigService remoteConfigService,
            IAppService drawingService,
            AppConfiguration appConfiguration
            )
        {
            _appConfiguration = appConfiguration;
            _configuration = configuration;
            _logger = logger;
            _azureStorageService = azureStorageService;
            _firestoreService = firestoreService;
            _remoteConfigService = remoteConfigService;
            _drawingService = drawingService;
        }


        [FunctionName("FunctionExport")]
        public async Task Run(
#if DEBUG
        [TimerTrigger("0 */1 * * * *")]
#else
        [TimerTrigger("0 30 12 */1 * *")] // Every day at 12:30 UTC
#endif
            TimerInfo myTimer
            )
        {
            try
            {
                _logger.LogInformation("Iniciando Aplicación de Exportación");

                var excelService = new ExcelService(_configuration, _logger);

                _logger.LogInformation("Configurando EPPlus");
                ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

                _logger.LogInformation("Leyendo documentos desde Firestore");
                List<Drawing> listDrawings;

#if DEBUG
                listDrawings = new List<Drawing>(){
                    await _firestoreService.FindDrawingByIdAsync("cloud", updateViews: false)
                };
#else
                listDrawings = await _firestoreService.GetDrawingsAsync();
#endif

                // TODO: resolver fallo en Producción aquí
                //_logger.LogInformation("Calculando Popularidad");
                //listDrawings = await _firestoreService.CalculatePopularityOfListDrawings(listDrawings);

                _logger.LogInformation("Procediendo a crear Excel");

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
                    using (var memoryStream = new MemoryStream())
                    {
                        var fileName = excelService.GetFileName();

                        excel.SaveAs(memoryStream);

                        memoryStream.Position = 0; // Restablecer la posición del Stream al inicio

                        try
                        {
                            await _azureStorageService.GuardarExcelEnAzureStorage(memoryStream, _appConfiguration.AzureStorage.ExportLocation, fileName);
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
