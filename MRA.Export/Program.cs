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

class Program
{
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
            var configuration = builder.Build();

            var firebaseHelper = new FirebaseHelper(configuration);
            var excelService = new ExcelService(configuration);

            // Configuración de EPPlus
            helper.ShowMessageInfo("Configurando EPPlus");
            ExcelPackage.LicenseContext = (LicenseContext)Enum.Parse(typeof(LicenseContext), excelService.License);

            // Configuración de Firestore
            helper.ShowMessageInfo("Registrando credenciales de Firebase");
            firebaseHelper.LoadCredentials();

            var projectId = firebaseHelper.ProjectId;
            var firestoreService = new FirestoreService(projectId, configuration["AzureStorage:BlobPath"]);
            firestoreService.SetCollectionNames(
                firebaseHelper.CollectionDrawings,
                firebaseHelper.CollectionCollections,
                firebaseHelper.CollectionInspirations
                );
            
            var remoteConfigService = new RemoteConfigService(new MemoryCache(new MemoryCacheOptions()), projectId, firebaseHelper.CredentialsPath, 60000);
            firestoreService.SetRemoteConfigService(remoteConfigService);

            helper.ShowMessageInfo("Recuperando documentos desde Firestore");
            var listDrawings = await firestoreService.GetDrawingsAsync();
            listDrawings = await firestoreService.CalculatePopularityOfListDrawings(listDrawings);

            // Crear un nuevo archivo Excel
            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add(excelService.SheetName);
                workSheet.View.FreezePanes(2, 2);

                var drawingProperties = ExcelService.GetPropertiesAttributes<Drawing>();

                excelService.SetTableHeaders(ref workSheet, drawingProperties);
                excelService.FillTable(ref workSheet, drawingProperties, listDrawings.OrderBy(x => x.Id).ToList());

                helper.ShowMessageInfo("Preparando formato de Tabla");
                ExcelService.CreateTable(ref workSheet, excelService.TableName, 1, 1, listDrawings.Count + 1, drawingProperties.Count);
                ExcelService.StyleCellsHeader(ref workSheet, 1, 1, 1, drawingProperties.Count);
                ExcelService.SetBold(ref workSheet, 2, 1, listDrawings.Count + 1, 1);

                helper.ShowMessageInfo("Preparando fichero para guardar");
                var fileInfo = excelService.GetFileInfo();
                excel.SaveAs(fileInfo);

                helper.ShowMessageInfo("Archivo Excel creado: " + fileInfo.FullName);
            }

        }catch(Exception ex)
        {
            helper.ShowMessageError("Ha ocurrido un error: " + ex.Message);
            helper.ShowMessageInfo("Pulse cualquier tecla para continuar");
            Console.ReadKey();
        }
    }
}