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

var console = new ConsoleHelper();
console.ShowMessageInfo("Cargando la configuración de la aplicación");
try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    var excelService = new ExcelService(configuration);

    // Configuración de EPPlus
    console.ShowMessageInfo("Configurando EPPlus");
    ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

    // Configuración de Firestore
    console.ShowMessageInfo("Registrando credenciales de Firebase");

    var firestoreService = new FirestoreService(configuration);

    //var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), firestoreService.ProjectId, firestoreService.CredentialsPath, 60000);
    //firestoreService.SetRemoteConfigService(remoteConfigService);

    console.ShowMessageInfo("Recuperando documentos desde Firestore");
    var listDrawingsFirestore = await firestoreService.GetDrawingsAsync();
    //listDrawingsFirestore = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

    console.ShowMessageInfo("Recuperando documentos desde documento Excel");
    //var filePath = console.FillStringValue("File to Read");
    var filePath = "M:\\Descargas\\Excels_MiguelRomerART\\test_add.xlsx";

    var fileInfo = new FileInfo(filePath);
    //var listDrawings = excelService.ImportDrawingsFromExcel(fileInfo);
    var listDrawingsProcessed = new List<Drawing>();

    console.ShowMessageInfo("Reading Automatic Commands");
    bool updateEverythingFromExcel = configuration.GetValue<bool>("Commands:UpdateEverythingFromExcel");
    console.ShowMessageInfo($"Update Everything From Excel: {(updateEverythingFromExcel ? "Yes" : "No")}");

    using (var package = new ExcelPackage(fileInfo))
    {
        var workSheet = package.Workbook.Worksheets[excelService.SheetName];
        if (workSheet == null)
        {
            throw new Exception($"Worksheet '{excelService.SheetName}' not found in the file.");
        }

        // Obtener propiedades de Drawing con atributos de Excel
        var drawingProperties = excelService.GetPropertiesAttributes<Drawing>();
        Dictionary<string, int> nameToColumnMap = excelService.GetColumnMapDrawing(workSheet);

        // Leer cada fila (comenzando en la segunda fila, asumiendo encabezado en la primera)
        int row = 2;
        while (workSheet.Cells[row, 1].Value != null) // Suponiendo que siempre hay un valor en la primera columna
        {
            bool error = false;
            console.ShowMessageInfo($"------------------------------------------------------");
            console.ShowMessageInfo($"Row {row}.");
            Drawing drawingExcel = excelService.ReadDrawingFromRow(workSheet, drawingProperties, nameToColumnMap, row);
            drawingExcel.Date = Utilities.GetStringFromDate(drawingExcel.DateObject);

            Drawing? drawingFirestore = listDrawingsFirestore.Find(x => x.Id == drawingExcel.Id);

            if (drawingFirestore != null)
            {
                // Edit document
                try
                {
                    console.ShowMessageInfo($"Updating Drawing \"{drawingExcel.Id}\"...");

                    foreach (var prop in drawingProperties.Where(x => x.Property.CanWrite))
                    {
                        if (!prop.Attribute.IgnoreOnImport && !prop.SameValues(drawingExcel, drawingFirestore))
                        {
                            bool updateValue = false;
                                console.ShowMessageWarning($"[{drawingExcel.Id}] Different value found for property \"{prop.Attribute.Name.ToUpper()}\":");

                                console.ShowMessageType = false;
                                console.ShowMessageWarning($"[{drawingExcel.Id}][FIRESTORE]:");
                                console.ShowMessageWarning(prop.GetValueToPrint(drawingFirestore));
                                console.ShowMessageWarning("");
                                console.ShowMessageWarning($"[{drawingExcel.Id}][EXCEL]:");
                                console.ShowMessageWarning(prop.GetValueToPrint(drawingExcel));


                            if (updateEverythingFromExcel)
                            {
                                console.ShowMessageInfo("Updating from Excel as it was commanded");
                                updateValue = true;
                            }
                            else
                            {
                                updateValue = console.FillBoolValue($"Update Firestore value for Excel Value for \"{prop.Attribute.Name}\"?");
                            }

                            if (updateValue)
                            {
                                prop.Property.SetValue(drawingFirestore, prop.GetValue(drawingExcel));
                                console.ShowMessageInfo($"Updated {drawingExcel.Id}.{prop.Attribute.Name} with:");
                                console.ShowMessageInfo($"{prop.GetValueToPrint(drawingExcel)}");
                            }
                            else
                            {
                                console.ShowMessageInfo($"{drawingExcel.Id}.{prop.Attribute.Name} sticks with:");
                                console.ShowMessageInfo($"{prop.GetValueToPrint(drawingFirestore)}");
                            }

                            console.ShowMessageType = true;
                        }
                    }

                    var newDrawingSaved = await firestoreService.AddDrawingAsync(drawingFirestore);
                    console.ShowMessageSuccess($"Drawing \"{newDrawingSaved.Id}\" was updated successfully.");
                }
                catch (Exception ex)
                {
                    console.ShowMessageError($"Could not save drawing with ID \"{drawingExcel.Id}\": {ex.Message}");
                    error = true;
                }
            }
            else
            {
                // New Document
                try
                {
                    console.ShowMessageInfo($"Drawing \"{drawingExcel.Id}\" not found in Firestore. Creating...");
                    var newDrawingSaved = await firestoreService.AddDrawingAsync(drawingExcel);
                    console.ShowMessageSuccess($"Drawing \"{newDrawingSaved.Id}\" was created successfully.");
                }
                catch (Exception ex)
                {
                    console.ShowMessageError($"Could not save drawing with ID \"{drawingExcel.Id}\": {ex.Message}");
                    error = true;
                }
            }

            if (!error && !listDrawingsProcessed.Any(x => x.Id == drawingExcel.Id))
            {
                listDrawingsProcessed.Add(drawingExcel);
            }

            console.ShowMessageInfo($"Press any key to continue reading file.");
            Console.ReadKey();

            row++;
        }
    }

    console.ShowMessageInfo($"List of Processed Drawings: {listDrawingsProcessed.Count}");
}
catch (Exception ex)
{
    console.ShowMessageError("Ha ocurrido un error: " + ex.Message);
}
console.ShowMessageInfo("Pulse cualquier tecla para continuar");
Console.ReadKey();