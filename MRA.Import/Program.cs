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
using MRA.DTO;
using MRA.DTO.Logger;

var console = new ConsoleHelper();
MRLogger? logger = null;
try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    logger = new MRLogger(configuration, console);
    logger.Log("Iniciando Aplicación de Importación");

    var excelService = new ExcelService(configuration, logger);

    logger.Log("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

    logger.Log("Registrando credenciales de Firebase");
    var firestoreService = new FirestoreService(configuration);

    if (!logger.ProductionEnvironmentAlert(firestoreService.IsInProduction))
    {
        return;
    }

    //var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    //firestoreService.SetRemoteConfigService(remoteConfigService);

    logger.Log("Recuperando documentos desde Firestore");
    var listDrawingsFirestore = await firestoreService.GetDrawingsAsync();
    //listDrawingsFirestore = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

    var filePath = console.FillStringValue("Ruta del Fichero a Procesar");
    //var filePath = "M:\\Descargas\\Excels_MiguelRomerART\\FirestoreDrawings_20241031_1038.xlsx";
    //var filePath = "M:\\Descargas\\Excels_MiguelRomerART\\test_add.xlsx";
    logger.Log($"Recuperando documentos desde Excel \"{filePath}\"");

    var fileInfo = new FileInfo(filePath);
    var listDrawingsProcessed = new List<Drawing>();
    var listDrawingsSaved = new List<Drawing>();
    var listDrawingsError = new Dictionary<int, Drawing>();

    logger.Log("Leyendo comandos automáticos");
    bool updateEverythingFromExcel = configuration.GetValue<bool>("Commands:UpdateEverythingFromExcel");
    if (updateEverythingFromExcel)
    {
        logger.Warning("Se sobreescribirán todos los cambios en Firestore con los datos del Excel");
    }
    else
    {
        logger.Info("Se preguntará al usuario en caso de cambios");
    }

    using (var package = new ExcelPackage(fileInfo))
    {
        logger.Log($"Leyendo hoja principal \"{excelService.SheetName}\"");
        var workSheet = package.Workbook.Worksheets[excelService.SheetName];
        if (workSheet == null)
        {
            throw new Exception($"Worksheet '{excelService.SheetName}' not found in the file.");
        }

        logger.Log("Obteniendo propiedades del DTO de Drawing");
        var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();

        logger.Log("Obteniendo mapeo entre nombres y números de columnas en el Excel");
        Dictionary<string, int> nameToColumnMap = excelService.GetColumnMapDrawing(workSheet);

        // Leer cada fila (comenzando en la segunda fila, asumiendo encabezado en la primera)
        int row = 2;
        while (workSheet.Cells[row, 1].Value != null) // Suponiendo que siempre hay un valor en la primera columna
        {
            bool error = false;
            logger.Log($"Leyendo fila {row}");
            Drawing drawingExcel = excelService.ReadDrawingFromRow(workSheet, drawingProperties, nameToColumnMap, row);

            logger.Log($"Dibujo \"{drawingExcel.Id}\" leído desde Excel");
            drawingExcel.Date = Utilities.GetStringFromDate(drawingExcel.DateObject);

            Drawing? drawingFirestore = listDrawingsFirestore.Find(x => x.Id == drawingExcel.Id);

            if (drawingFirestore != null)
            {
                try
                {
                    bool existenCambios = false;
                    logger.Info($"Existe un dibujo \"{drawingFirestore.Id}\" en Firestore. Procediendo a EDITAR");

                    foreach (var prop in drawingProperties.Where(x => x.Property.CanWrite))
                    {
                        if (!prop.Attribute.IgnoreOnImport && !prop.SameValues(drawingExcel, drawingFirestore))
                        {
                            bool updateValue = false;
                            logger.CleanLog("-------------------------");
                            logger.Warning($"\"{drawingExcel.Id}\" tiene diferentes valores para \"{prop.Attribute.Name.ToUpper()}\".");

                            logger.Error("En FIRESTORE:", showTime: false, showPrefix: false);
                            logger.Error(prop.GetValueToPrint(drawingFirestore), showTime: false, showPrefix: false);
                            logger.CleanLog(".........................");
                            logger.Success("En EXCEL:", showTime: false, showPrefix: false);
                            logger.Success(prop.GetValueToPrint(drawingExcel), showTime: false, showPrefix: false);

                            updateValue = updateEverythingFromExcel
                                ? true
                                : console.FillBoolValue($"Actualizar valor de Firestore con el valor del Excel para \"{prop.Attribute.Name.ToUpper()}\"?");

                            if (updateValue)
                            {
                                existenCambios = true;
                                logger.Info("Actualizando valor con EXCEL:");
                                logger.Success(prop.GetValueToPrint(drawingExcel), showTime: false, showPrefix: false);
                                prop.Property.SetValue(drawingFirestore, prop.GetValue(drawingExcel));
                            }
                            else
                            {
                                logger.Info("Permanece el valor de FIRESTORE:");
                                logger.Error(prop.GetValueToPrint(drawingFirestore), showTime: false, showPrefix: false);
                            }
                        }
                    }

                    if (existenCambios) { 
                        logger.Info($"Actualizando \"{drawingFirestore.Id}\" en BBDD");
                        var updatedDrawingSaved = await firestoreService.AddDrawingAsync(drawingFirestore);
                        logger.Success($"\"{updatedDrawingSaved.Id}\" guardado con éxito");

                        if (!listDrawingsSaved.Any(x => x.Id == updatedDrawingSaved.Id))
                        {
                            listDrawingsSaved.Add(updatedDrawingSaved);
                        }
                    }
                    else
                    {
                        logger.Info($"No se han detectado cambios en \"{drawingFirestore.Id}\" respecto al Excel. Se ignora");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"No se pudo actualizar el dibujo \"{drawingExcel.Id}\": {ex.Message}");
                    error = true;
                }
            }
            else
            {
                // New Document
                try
                {
                    logger.Warning($"No existe ningún dibujo con ID \"{drawingExcel.Id}\"");
                    var newDrawingSaved = await firestoreService.AddDrawingAsync(drawingExcel);
                    logger.Success($"Dibujo \"{newDrawingSaved.Id}\" creado con éxito");

                    if (!listDrawingsSaved.Any(x => x.Id == newDrawingSaved.Id))
                    {
                        listDrawingsSaved.Add(newDrawingSaved);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"No se pudo crear el dibujo \"{drawingExcel.Id}\": {ex.Message}");
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
                console.ShowMessageInfo($"Pulsa cualquier tecla para continuar con la siguiente línea. Actual: {row}.");
                Console.ReadKey();
            }

            row++;
        }
    }

    logger.Info($"Dibujos En Firestore: {listDrawingsFirestore.Count}.");
    logger.Info($"Dibujos Procesados: {listDrawingsProcessed.Count}.");
    logger.Success($"Guardados: {listDrawingsSaved.Count}.");
    logger.Error($"Errores: {listDrawingsError.Count}.");
    if (listDrawingsError.Any())
    {
        foreach (var error in listDrawingsError)
        {
            logger.Error($"Fila {error.Key}, \"{error.Value.Id}", showTime: false);
        }
    }
}
catch (Exception ex)
{
    logger?.Error("Ha ocurrido un error: " + ex.Message);
}
console.ShowMessageInfo("Pulse cualquier tecla para continuar");
Console.ReadKey();