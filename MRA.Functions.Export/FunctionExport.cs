using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Models;
using MRA.Infrastructure.Configuration;
using MRA.Services;
using MRA.Services.AzureStorage;
using MRA.Services.Excel;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Firebase.RemoteConfig;
using MRA.Services.Models.Drawings;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Style.Effect;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        private readonly ILogger<FunctionExport> _logger;
        private readonly IExcelService _excelService;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IDrawingService _drawingService;
        private readonly IAppService _appService;
        private readonly AppConfiguration _appConfiguration;

        public FunctionExport(
            ILogger<FunctionExport> logger,
            IExcelService excelService,
            IAzureStorageService azureStorageService,
            IRemoteConfigService remoteConfigService,
            IDrawingService drawingService,
            IAppService appService,
            AppConfiguration appConfiguration
            )
        {
            _appConfiguration = appConfiguration;
            _excelService = excelService;
            _logger = logger;
            _azureStorageService = azureStorageService;
            _remoteConfigService = remoteConfigService;
            _drawingService = drawingService;
            _appService = appService;
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

                _logger.LogInformation("Configurando EPPlus");
                ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), _excelService.GetEPPlusLicense());

                _logger.LogInformation("Leyendo documentos desde Firestore");
                List<DrawingModel> listDrawings;

#if DEBUG
                listDrawings = new List<DrawingModel>(){
                    await _drawingService.FindDrawingAsync("cloud", onlyIfVisible: false, updateViews: false)
                };
#else
                listDrawings = (await _drawingService.GetAllDrawingsAsync(onlyIfVisible: false)).ToList();
#endif

                // TODO: resolver fallo en Producción aquí
                _logger.LogInformation("Calculando Popularidad");
                listDrawings = (await _appService.CalculatePopularityOfListDrawings(listDrawings)).ToList();

                _logger.LogInformation("Procediendo a crear Excel");

                using (ExcelPackage excel = new ExcelPackage())
                {
                    _logger.LogInformation($"Creando hoja principal \"{ExcelService.EXCEL_DRAWING_SHEET_NAME}\"");
                    var workSheet = excel.Workbook.Worksheets.Add(ExcelService.EXCEL_DRAWING_SHEET_NAME);
                    workSheet.View.FreezePanes(2, 2);

                    _logger.LogInformation("Obteniendo propiedades del DTO de Drawing");
                    var drawingProperties = _excelService.GetPropertiesAttributes<DrawingModel>();

                    _logger.LogInformation("Rellenando tabla principal");
                    _excelService.FillDrawingTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

                    _logger.LogInformation("Preparando hojas de diccionarios");
                    _excelService.FillSheetsDictionary(excel, drawingProperties, workSheet);

                    _logger.LogInformation("Preparando fichero para guardar en Azure Storage");
                    using (var memoryStream = new MemoryStream())
                    {
                        var fileName = _excelService.GetFileName();

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
