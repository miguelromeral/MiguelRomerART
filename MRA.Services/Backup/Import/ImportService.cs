using Microsoft.Extensions.Logging;
using MRA.DTO;
using MRA.DTO.Models;
using MRA.Infrastructure.Enums;
using MRA.Infrastructure.Excel.Attributes;
using MRA.Infrastructure.Settings;
using MRA.Services.Excel;
using MRA.Services.Excel.Interfaces;
using MRA.Services.Models.Drawings;
using MRA.Services.UserInput;
using OfficeOpenXml;
using System.ComponentModel;
using System.Reflection;

namespace MRA.Services.Backup.Import;

public class ImportService : IImportService
{
    private readonly IExcelService excelService;
    private readonly IDrawingService drawingService;
    private readonly IUserInputService inputService;
    private readonly AppSettings appSettings;
    private readonly ILogger<ImportService> logger;

    public ImportService(
        IExcelService excelService,
        IDrawingService drawingService,
        IUserInputService inputService,
        AppSettings appSettings,
        ILogger<ImportService> logger)
    {
        this.excelService = excelService;
        this.drawingService = drawingService;
        this.inputService = inputService;
        this.appSettings = appSettings;
        this.logger = logger;
    }

    public async Task ImportDrawings()
    {
        logger.LogInformation("Iniciando Aplicación de Importación");

        logger.LogInformation("Recuperando documentos");
        var listDrawings = await drawingService.GetAllDrawingsAsync(onlyIfVisible: false);

        var filePath = inputService.ReadStringValue("Ruta del Fichero a Procesar");
        logger.LogInformation("Recuperando documentos desde Excel '{FilePath}'", filePath);

        var fileInfo = new FileInfo(filePath);
        var listDrawingsProcessed = new List<DrawingModel>();
        var listDrawingsSaved = new List<DrawingModel>();
        var listDrawingsError = new Dictionary<int, DrawingModel>();

        logger.LogInformation("Leyendo comandos automáticos");
        bool updateEverythingFromExcel = appSettings.Commands.UpdateEverythingFromExcel;
        if (updateEverythingFromExcel)
        {
            logger.LogWarning("Se sobreescribirán todos los cambios en Firestore con los datos del Excel");
        }
        else
        {
            logger.LogInformation("Se preguntará al usuario en caso de cambios");
        }

        using (var package = new ExcelPackage(fileInfo))
        {
            logger.LogInformation("Leyendo hoja principal '{Sheet}'", ExcelService.EXCEL_DRAWING_SHEET_NAME);
            var workSheet = package.Workbook.Worksheets[ExcelService.EXCEL_DRAWING_SHEET_NAME];
            if (workSheet == null)
            {
                throw new Exception($"Worksheet '{ExcelService.EXCEL_DRAWING_SHEET_NAME}' not found in the file.");
            }

            logger.LogInformation("Obteniendo propiedades del DTO de Drawing");
            var drawingProperties = excelService.GetPropertiesAttributes<DrawingModel>();

            logger.LogInformation("Obteniendo mapeo entre nombres y números de columnas en el Excel");
            Dictionary<string, int> nameToColumnMap = excelService.GetColumnMapDrawing(workSheet);

            int row = 2;
            while (workSheet.Cells[row, 1].Value != null)
            {
                bool error = false;
                logger.LogInformation("Leyendo fila {Row}", row);
                DrawingModel drawingExcel = ReadDrawingFromRow(workSheet, drawingProperties, nameToColumnMap, row);

                logger.LogInformation("Dibujo '{Id}' leído desde Excel", drawingExcel.Id);
                drawingExcel.Date = Utilities.GetStringFromDate(drawingExcel.DateObject);

                DrawingModel? drawingDatabase = listDrawings.FirstOrDefault(x => x.Id == drawingExcel.Id);

                if (drawingDatabase != null)
                {
                    try
                    {
                        bool differentValues = false;
                        logger.LogInformation("Existe un dibujo '{Id}'. Procediendo a EDITAR", drawingDatabase.Id);

                        foreach (var prop in drawingProperties.Where(x => x.Property.CanWrite))
                        {
                            if (!prop.Attribute.IgnoreOnImport && !prop.SameValues(drawingExcel, drawingDatabase))
                            {
                                bool updateValue = false;
                                logger.LogInformation("-------------------------");
                                logger.LogWarning("'{Id}' tiene diferentes valores para '{Property}'.", drawingExcel.Id, prop.Attribute.Name.ToUpper());

                                logger.LogError("En BBDD: {Value}", prop.GetValueToPrint(drawingDatabase));
                                logger.LogWarning("En EXCEL: {Value}", prop.GetValueToPrint(drawingExcel));
                                
                                updateValue = updateEverythingFromExcel || inputService.ReadBoolValue($"Actualizar valor de Firestore con el valor del Excel para \"{prop.Attribute.Name.ToUpper()}\"?");

                                if (updateValue)
                                {
                                    differentValues = true;
                                    logger.LogInformation("Actualizando valor con EXCEL: {Value}", prop.GetValueToPrint(drawingExcel));
                                    prop.Property.SetValue(drawingDatabase, prop.GetValue(drawingExcel));
                                }
                                else
                                {
                                    logger.LogInformation("Permanece el valor de FIRESTORE: {Value}", prop.GetValueToPrint(drawingDatabase));
                                }
                            }
                        }

                        if (differentValues)
                        {
                            logger.LogInformation("Actualizando '{Id}' en BBDD", drawingDatabase.Id);
                            var updatedDrawingSaved = await drawingService.SaveDrawingAsync(drawingDatabase);
                            logger.LogInformation("'{Id}' guardado con éxito", updatedDrawingSaved.Id);

                            if (!listDrawingsSaved.Any(x => x.Id == updatedDrawingSaved.Id))
                            {
                                listDrawingsSaved.Add(updatedDrawingSaved);
                            }
                        }
                        else
                        {
                            logger.LogInformation("No se han detectado cambios en '{Id}' respecto al Excel. Se ignora", drawingDatabase.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "No se pudo actualizar el dibujo '{Id}'.", drawingExcel.Id);
                        error = true;
                    }
                }
                else
                {
                    try
                    {
                        logger.LogWarning("No existe ningún dibujo con ID '{Id}'", drawingExcel.Id);
                        var newDrawingSaved = await drawingService.SaveDrawingAsync(drawingExcel);
                        logger.LogInformation("Dibujo '{Id}' creado con éxito", newDrawingSaved.Id);

                        if (!listDrawingsSaved.Any(x => x.Id == newDrawingSaved.Id))
                        {
                            listDrawingsSaved.Add(newDrawingSaved);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "No se pudo crear el dibujo '{Id}'.", drawingExcel.Id);
                        error = true;
                    }
                }

                if (!error && !listDrawingsProcessed.Any(x => x.Id == drawingExcel.Id))
                {
                    listDrawingsProcessed.Add(drawingExcel);
                }

                if (error)
                {
                    listDrawingsError.Add(row, drawingExcel);
                }

                if (!updateEverythingFromExcel)
                {
                    logger.LogInformation("Pulsa cualquier tecla para continuar con la siguiente línea. Actual: {Row}.", row);
                    inputService.ReadKey();
                }

                row++;
            }
        }

        logger.LogInformation("Dibujos En Firestore: {Count}.", listDrawings.Count());
        logger.LogInformation("Dibujos Procesados: {Count}.", listDrawingsProcessed.Count);
        logger.LogInformation("Guardados: {Count}.", listDrawingsSaved.Count);
        logger.LogError("Errores: {Count}.", listDrawingsError.Count);
        if (listDrawingsError.Any())
        {
            foreach (var error in listDrawingsError)
            {
                logger.LogError("Fila {Key}, '{Id}'", error.Key, error.Value.Id);
            }
        }
    }

    private static DrawingModel ReadDrawingFromRow(ExcelWorksheet workSheet, List<ExcelColumnInfo> drawingProperties, Dictionary<string, int> nameToColumnMap, int row)
    {
        var drawing = new DrawingModel();

        foreach (var propInfo in drawingProperties.Where(x => x.Property.CanWrite))
        {
            if (!nameToColumnMap.TryGetValue(propInfo.Attribute.Name, out int col))
            {
                continue;
            }

            //throw new NotImplementedException("TODO: fix reading value null in boolean and numbers.");

            var cellValue = workSheet.GetValue(row, col);
            SetPropertyValue(drawing, propInfo.Property, cellValue);
        }

        return drawing;
    }

    private static void SetPropertyValue(DrawingModel drawing, PropertyInfo property, object cellValue)
    {
        if (cellValue == null)
        {
            property.SetValue(drawing, null);
            return;
        }

        Type propType = property.PropertyType;

        try
        {
            if (propType == typeof(string))
            {
                property.SetValue(drawing, cellValue.ToString());
            }
            else if (propType == typeof(int))
            {
                property.SetValue(drawing, Convert.ToInt32(cellValue));
            }
            else if (propType == typeof(long))
            {
                property.SetValue(drawing, Convert.ToInt64(cellValue));
            }
            else if (propType == typeof(double))
            {
                property.SetValue(drawing, Convert.ToDouble(cellValue));
            }
            else if (propType == typeof(decimal))
            {
                property.SetValue(drawing, Convert.ToDecimal(cellValue));
            }
            else if (propType == typeof(DateTime))
            {
                property.SetValue(drawing, DateTime.FromOADate(Convert.ToDouble(cellValue)));
            }
            else if (propType == typeof(bool))
            {
                switch (property.Name)
                {
                    case "Visible":
                        property.SetValue(drawing, cellValue.ToString() == ExcelService.EXCEL_VISIBLE_VALUE);
                        break;
                    case "Favorite":
                        property.SetValue(drawing, cellValue.ToString() == ExcelService.EXCEL_FAVORITE_VALUE);
                        break;
                    default:
                        property.SetValue(drawing, cellValue.ToString() == "TRUE");
                        break;
                }
            }
            else if (propType == typeof(IEnumerable<string>))
            {
                List<string> list;
                switch (property.Name)
                {
                    case "Tags":
                        list = cellValue.ToString().Split(new[] { DrawingTagManager.TAG_SEPARATOR }, StringSplitOptions.None).ToList();
                        break;
                    default:
                        list = cellValue.ToString().Split(new[] { ExcelService.EXCEL_SEPARATOR_COMMENTS }, StringSplitOptions.None).ToList();
                        break;
                }

                property.SetValue(drawing, list);
            }
            else if (propType == typeof(Uri))
            {
                if (Uri.IsWellFormedUriString(cellValue.ToString(), UriKind.Absolute))
                {
                    property.SetValue(drawing, new Uri(cellValue.ToString()));
                }
            }
            else if (propType.IsEnum) // Manejo de enumeraciones
            {
                string stringValue = cellValue.ToString();

                // Intentar mapear al enum por nombre
                if (Enum.TryParse(propType, stringValue, true, out var enumValue))
                {
                    property.SetValue(drawing, enumValue);
                    return;
                }

                // Intentar mapear al enum por descripción
                foreach (var field in propType.GetFields())
                {
                    var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                    if (descriptionAttribute != null && descriptionAttribute.Description.Equals(stringValue, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var enumParsedValue = Enum.Parse(propType, field.Name);
                        property.SetValue(drawing, enumParsedValue);
                        return;
                    }
                }

                // Si no se puede mapear, asignar valor predeterminado (si existe)
                var defaultEnumValueAttribute = propType.GetCustomAttribute<DefaultEnumValueAttribute>();
                if (defaultEnumValueAttribute != null)
                {
                    property.SetValue(drawing, defaultEnumValueAttribute.DefaultValue);
                }
                else
                {
                    property.SetValue(drawing, Activator.CreateInstance(propType));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting property {property.Name}: {ex.Message}");
        }
    }
}
