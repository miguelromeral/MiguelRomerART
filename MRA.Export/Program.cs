using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using MRA.Services.Helpers;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using MRA.Services.Firebase;

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
            FirebaseHelper.SetCredentialsLocally(@".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

            var projectId = _configuration["Firebase:ProjectID"];
            string collectionName = _configuration["Firebase:CollectionDrawings"];
            FirestoreDb db = FirestoreDb.Create(projectId);
            //var firestoreService = new FirestoreService(projectId, _configuration["AzureStorage:BlobPath"]);

            helper.ShowMessageInfo("Recuperando documentos desde Firestore");

            // Obtener documentos de Firestore
            Query query = db.Collection(collectionName);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            // Crear un nuevo archivo Excel
            using (ExcelPackage excel = new ExcelPackage())
            {
                var sheetName = _configuration["Excel:SheetName"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                workSheet.Cells[1, 1].Value = "ID";
                workSheet.Cells[1, 2].Value = "Name";
                workSheet.Cells[1, 3].Value = "Path";
                // Agrega más encabezados según tus propiedades

                int row = 2;
                int numberDocuments = snapshot.Documents.Count;
                foreach (var document in snapshot.Documents)
                {
                    helper.ShowMessageInfo($"Procesando documento ({row - 1}/{numberDocuments}): "+document.Id);
                    workSheet.Cells[row, 1].Value = document.Id;
                    workSheet.Cells[row, 2].Value = document.GetValue<string>("name");
                    workSheet.Cells[row, 3].Value = document.GetValue<string>("path");
                    row++;
                }
                
                helper.ShowMessageInfo("Preparando formato de Tabla");

                // Crear el rango de la tabla
                var dataRange = workSheet.Cells[1, 1, row - 1, 3];
                var tableName = _configuration["Excel:Table:Name"];
                var table = workSheet.Tables.Add(dataRange, tableName);
                table.TableStyle = TableStyles.Medium6;

                // Dar formato de color a la primera fila (encabezado)
                ExcelHelper.StyleCellsHeader(ref workSheet, 1, 1, 1, 3);

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