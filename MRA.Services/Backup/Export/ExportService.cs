using Microsoft.Extensions.Logging;
using MRA.DTO.Models;
using MRA.Infrastructure.Settings;
using MRA.Services.Excel;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Models.Drawings;
using MRA.Services.RemoteConfig;
using MRA.Services.Storage;
using OfficeOpenXml;

namespace MRA.Services.Backup.Export;

public class ExportService : IExportService
{
    private readonly ILogger<ExportService> _logger;
    private readonly IExcelService _excelService;
    private readonly IStorageService _storageService;
    private readonly IDrawingService _drawingService;
    private readonly IAppService _appService;
    private readonly AppSettings _appConfiguration;

    public ExportService(
        ILogger<ExportService> logger,
        IExcelService excelService,
        IStorageService storageService,
        IRemoteConfigService remoteConfigService,
        IDrawingService drawingService,
        IAppService appService,
        AppSettings appConfiguration
        )
    {
        _appConfiguration = appConfiguration;
        _excelService = excelService;
        _logger = logger;
        _storageService = storageService;
        _drawingService = drawingService;
        _appService = appService;
    }


    public async Task ExportDrawings()
    {
        try
        {
            _logger.LogInformation("Iniciando Aplicación de Exportación");


            _logger.LogInformation("Leyendo documentos desde Firestore");
            List<DrawingModel> listDrawings;
            listDrawings = (await _drawingService.GetAllDrawingsAsync(onlyIfVisible: false)).ToList();

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

                    await excel.SaveAsAsync(memoryStream);

                    memoryStream.Position = 0; // Restablecer la posición del Stream al inicio

                    try
                    {
                        var success = await _storageService.Save(memoryStream, _appConfiguration.AzureStorage.ExportLocation, fileName);
                        if (!success)
                        {
                            _logger.LogError("File not saved to Storage");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al guardar el archivo '{FileName}' en Azure Storage", fileName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la exportación");
        }
        _logger.LogInformation("Fin de la Exportación en Azure Functions");
    }
}
