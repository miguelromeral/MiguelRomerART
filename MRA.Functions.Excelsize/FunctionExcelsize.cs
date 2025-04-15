using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Settings;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Models.Drawings;
using MRA.Services.RemoteConfig;
using MRA.Services.Storage;
using MRA.Services;
using MRA.DTO.Models;
using MRA.Services.Excel;
using OfficeOpenXml;

namespace MRA.Functions.Excelsize;

public class FunctionExcelsize
{
    //private readonly ILogger<FunctionExcelsize> _logger;
    private readonly IExcelService _excelService;
    private readonly IStorageService _storageService;
    private readonly IRemoteConfigService _remoteConfigService;
    private readonly IDrawingService _drawingService;
    private readonly IAppService _appService;
    private readonly AppSettings _appConfiguration;
    private readonly ILogger _logger;

    public FunctionExcelsize(
        ILoggerFactory loggerFactory,
        IExcelService excelService,
        IStorageService storageService,
        IRemoteConfigService remoteConfigService,
        IDrawingService drawingService,
        IAppService appService,
        AppSettings appConfiguration)
    {
        _logger = loggerFactory.CreateLogger<FunctionExcelsize>();
        _appConfiguration = appConfiguration;
        _excelService = excelService;
        //_logger = logger;
        _storageService = storageService;
        _remoteConfigService = remoteConfigService;
        _drawingService = drawingService;
        _appService = appService;
    }

    [Function("FunctionExcelsize")]
    public async Task Run(
#if DEBUG
    [TimerTrigger("0 */1 * * * *")]
#else
    [TimerTrigger("0 30 12 */1 * *")] // Every day at 12:30 UTC
#endif      
    TimerInfo myTimer)
    {
        try
        {
            _logger.LogInformation("Iniciando Aplicación de Exportación");


            _logger.LogInformation("Leyendo documentos desde Firestore");
            List<DrawingModel> listDrawings;

#if DEBUG
            listDrawings = new List<DrawingModel>(){
                await _drawingService.FindDrawingAsync("cloud", onlyIfVisible: false, updateViews: false)
            };
#else
            listDrawings = (await _drawingService.GetAllDrawingsAsync(onlyIfVisible: false)).ToList();
#endif

            _logger.LogInformation("Calculando Popularidad");
            listDrawings = _appService.CalculatePopularityOfListDrawings(listDrawings).ToList();

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
                        await _storageService.Save(memoryStream, _appConfiguration.AzureStorage.ExportLocation, fileName);
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
