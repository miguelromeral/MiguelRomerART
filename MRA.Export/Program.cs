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

class Program
{
    private static IConfiguration _configuration;

    static async Task Main(string[] args)
    {
        var helper = new ConsoleHelper();
        helper.ShowMessageInfo("Cargando la configuración de la aplicación");
        try
        {
            // Configuración de la aplicación
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();


            // Configuración de EPPlus
            helper.ShowMessageInfo("Configurando EPPlus");
            var epplusLicenseContext = _configuration["EPPlus:ExcelPackage:LicenseContext"];
            ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), epplusLicenseContext);

            // Configuración de Firestore
            helper.ShowMessageInfo("Registrando credenciales de Firebase");
            var credentialsPath = @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json";
            FirebaseHelper.SetCredentialsLocally(credentialsPath);

            var projectId = _configuration["Firebase:ProjectID"];
            string collectionName = _configuration["Firebase:CollectionDrawings"];
            //FirestoreDb db = FirestoreDb.Create(projectId);
            var firestoreService = new FirestoreService(projectId, _configuration["AzureStorage:BlobPath"]);
            firestoreService.SetCollectionNames(
                _configuration["Firebase:CollectionDrawings"],
                _configuration["Firebase:CollectionCollections"],
                _configuration["Firebase:CollectionInspirations"]
                );
            
            var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), projectId, credentialsPath, 60000);
            firestoreService.SetRemoteConfigService(remoteConfigService);

            helper.ShowMessageInfo("Recuperando documentos desde Firestore");
            var listDrawings = await firestoreService.GetDrawingsAsync();
            listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

            // Crear un nuevo archivo Excel
            using (ExcelPackage excel = new ExcelPackage())
            {
                var sheetName = _configuration["Excel:SheetName"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                workSheet.View.FreezePanes(2, 2);

                var drawingProperties = ExcelHelper.GetPropertiesAttributes<Drawing>();

                int col = 1;
                foreach (var prop in drawingProperties)
                {
                    workSheet.Cells[1, col].Value = prop.Attribute.Name;
                    col++;
                }

                int numberDocuments = listDrawings.Count;
                int row = 2;
                foreach (var drawing in listDrawings.OrderBy(x => x.Id)) // drawingsList es la lista de tus objetos Drawing
                {
                    helper.ShowMessageInfo($"Procesando documento ({row - 1}/{numberDocuments}): "+drawing.Id);
                    col = 1;
                    foreach (var prop in drawingProperties)
                    {
                        workSheet.Cells[row, col].Value = prop.Property.GetValue(drawing); // Obtener el valor de la propiedad
                        col++;
                    }
                    row++;
                }

                helper.ShowMessageInfo("Preparando formato de Tabla");
                ExcelHelper.CreateTable(ref workSheet, _configuration["Excel:Table:Name"], 1, 1, row - 1, drawingProperties.Count);
                ExcelHelper.StyleCellsHeader(ref workSheet, 1, 1, 1, drawingProperties.Count);

                helper.ShowMessageInfo("Preparando fichero para guardar");

                var directoryPath = _configuration["Excel:File:Path"];
                var fileName = $"{_configuration["Excel:File:Name"]}" +
                    $"_{DateTime.Now.ToString(_configuration["Excel:File:DateFormat"])}" +
                    $".{_configuration["Excel:File:Extension"]}";
                var filePath = Path.Combine(directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    helper.ShowMessageWarning("Directorio no existente, procediendo a crear " + directoryPath);
                    Directory.CreateDirectory(directoryPath);
                }

                // Guardar el archivo Excel
                var fileInfo = new FileInfo(filePath);
                excel.SaveAs(fileInfo);
                helper.ShowMessageInfo("Archivo Excel creado: " + fileInfo.FullName);
            }

        }catch(Exception ex)
        {
            helper.ShowMessageError("Ha ocurrido un error: " + ex.Message);
        }

        helper.ShowMessageInfo("Pulse cualquier tecla para continuar");
        Console.ReadKey();
    }
}