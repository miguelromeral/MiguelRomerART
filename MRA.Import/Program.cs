﻿using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using MRA.Services.Helpers;
using Microsoft.Extensions.Caching.Memory;
using MRA.Services.Excel;
using MRA.DTO;
using MRA.DTO.Logger;
using MRA.DependencyInjection.Startup;
using Microsoft.Extensions.Logging;

var console = new ConsoleHelper();
ILogger? logger = null;
try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .AddCustomAppSettingsFiles("Development", isDevelopment: true)
        //.AddEnvironmentVariables()
        ;

    var configuration = builder.Build();

    var appConfig = configuration.GetMRAConfiguration();

    logger = new MRLogger(appConfig);
    logger.Log("Iniciando Aplicación de Importación");

    var excelService = new ExcelService(appConfig, logger);

    logger.Log("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.GetEPPlusLicense());

    logger.Log("Registrando credenciales de Firebase");
    var firestoreService = new FirestoreService(appConfig, new FirestoreDatabase(appConfig), null);

    //var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    //firestoreService.SetRemoteConfigService(remoteConfigService);

    logger.Log("Recuperando documentos desde Firestore");
    var listDrawingsFirestore = await firestoreService.GetDrawingsAsync();
    //listDrawingsFirestore = firestoreService.CalculatePopularityOfListDrawings(listDrawings);

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
        logger.LogWarning("Se sobreescribirán todos los cambios en Firestore con los datos del Excel");
    }
    else
    {
        logger.LogInformation("Se preguntará al usuario en caso de cambios");
    }

    using (var package = new ExcelPackage(fileInfo))
    {
        logger.Log($"Leyendo hoja principal \"{ExcelService.EXCEL_DRAWING_SHEET_NAME}\"");
        var workSheet = package.Workbook.Worksheets[ExcelService.EXCEL_DRAWING_SHEET_NAME];
        if (workSheet == null)
        {
            throw new Exception($"Worksheet '{ExcelService.EXCEL_DRAWING_SHEET_NAME}' not found in the file.");
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
                    logger.LogInformation($"Existe un dibujo \"{drawingFirestore.Id}\" en Firestore. Procediendo a EDITAR");

                    foreach (var prop in drawingProperties.Where(x => x.Property.CanWrite))
                    {
                        if (!prop.Attribute.IgnoreOnImport && !prop.SameValues(drawingExcel, drawingFirestore))
                        {
                            bool updateValue = false;
                            logger.CleanLog("-------------------------");
                            logger.LogWarning($"\"{drawingExcel.Id}\" tiene diferentes valores para \"{prop.Attribute.Name.ToUpper()}\".");

                            logger.LogError("En FIRESTORE:", showTime: false, showPrefix: false);
                            logger.LogError(prop.GetValueToPrint(drawingFirestore), showTime: false, showPrefix: false);
                            logger.CleanLog(".........................");
                            logger.LogInformation("En EXCEL:", showTime: false, showPrefix: false);
                            logger.LogInformation(prop.GetValueToPrint(drawingExcel), showTime: false, showPrefix: false);

                            updateValue = updateEverythingFromExcel
                                ? true
                                : console.FillBoolValue($"Actualizar valor de Firestore con el valor del Excel para \"{prop.Attribute.Name.ToUpper()}\"?");

                            if (updateValue)
                            {
                                existenCambios = true;
                                logger.LogInformation("Actualizando valor con EXCEL:");
                                logger.LogInformation(prop.GetValueToPrint(drawingExcel), showTime: false, showPrefix: false);
                                prop.Property.SetValue(drawingFirestore, prop.GetValue(drawingExcel));
                            }
                            else
                            {
                                logger.LogInformation("Permanece el valor de FIRESTORE:");
                                logger.LogError(prop.GetValueToPrint(drawingFirestore), showTime: false, showPrefix: false);
                            }
                        }
                    }

                    if (existenCambios) { 
                        logger.LogInformation($"Actualizando \"{drawingFirestore.Id}\" en BBDD");
                        var updatedDrawingSaved = await firestoreService.AddDrawingAsync(drawingFirestore);
                        logger.LogInformation($"\"{updatedDrawingSaved.Id}\" guardado con éxito");

                        if (!listDrawingsSaved.Any(x => x.Id == updatedDrawingSaved.Id))
                        {
                            listDrawingsSaved.Add(updatedDrawingSaved);
                        }
                    }
                    else
                    {
                        logger.LogInformation($"No se han detectado cambios en \"{drawingFirestore.Id}\" respecto al Excel. Se ignora");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"No se pudo actualizar el dibujo \"{drawingExcel.Id}\": {ex.Message}");
                    error = true;
                }
            }
            else
            {
                // New Document
                try
                {
                    logger.LogWarning($"No existe ningún dibujo con ID \"{drawingExcel.Id}\"");
                    var newDrawingSaved = await firestoreService.AddDrawingAsync(drawingExcel);
                    logger.LogInformation($"Dibujo \"{newDrawingSaved.Id}\" creado con éxito");

                    if (!listDrawingsSaved.Any(x => x.Id == newDrawingSaved.Id))
                    {
                        listDrawingsSaved.Add(newDrawingSaved);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"No se pudo crear el dibujo \"{drawingExcel.Id}\": {ex.Message}");
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

    logger.LogInformation($"Dibujos En Firestore: {listDrawingsFirestore.Count}.");
    logger.LogInformation($"Dibujos Procesados: {listDrawingsProcessed.Count}.");
    logger.LogInformation($"Guardados: {listDrawingsSaved.Count}.");
    logger.LogError($"Errores: {listDrawingsError.Count}.");
    if (listDrawingsError.Any())
    {
        foreach (var error in listDrawingsError)
        {
            logger.LogError($"Fila {error.Key}, \"{error.Value.Id}", showTime: false);
        }
    }
}
catch (Exception ex)
{
    logger?.LogError("Ha ocurrido un error: " + ex.Message);
}
console.ShowMessageInfo("Pulse cualquier tecla para continuar");
Console.ReadKey();