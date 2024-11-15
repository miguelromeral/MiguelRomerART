using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.AzureStorage;
using MRA.Services.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace MRA.Services.AzureStorage
{
    public class AzureStorageService
    {
        #region Environment Variables Names
        private const string ENV_AZURE_STORAGE_CONNECTION_STRING = "ENV_AZURE_STORAGE_CONNECTION_STRING";
        #endregion

        #region App Settings Names
        private const string APPSETTING_AZURE_URL_BASE = "AzureStorage:BlobPath";
        private const string APPSETTING_AZURE_BLOB_STORAGE_CONTAINER = "AzureStorage:BlobStorageContainer";
        private const string APPSETTING_AZURE_BLOB_STORAGE_EXPORT_LOCATION = "AzureStorage:Backup:Export:Location";
        #endregion

        public string BlobURL { get { return _configuration[APPSETTING_AZURE_URL_BASE]; } }
        public string BlobStorageContainer { get { return _configuration[APPSETTING_AZURE_BLOB_STORAGE_CONTAINER]; } }

        private readonly string _connectionString;
        public string ConnectionString { get { return _connectionString; } }
        public string ExportLocation { get { return _configuration[APPSETTING_AZURE_BLOB_STORAGE_EXPORT_LOCATION]; } }

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobServiceClient;


        public AzureStorageService(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = EnvironmentHelper.ReadValue(ENV_AZURE_STORAGE_CONNECTION_STRING);
            _blobServiceClient = new BlobServiceClient(ConnectionString);
        }

        public async Task<List<BlobFileInfo>> ListBlobFilesAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

            var blobFiles = new List<BlobFileInfo>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            //await foreach (var blobItem in containerClient.FindBlobsByTagsAsync(""))
            {
                blobFiles.Add(new BlobFileInfo
                {
                    Name = blobItem.Name,
                    Url = containerClient.Uri + "/" + blobItem.Name
                });
            }

            return blobFiles;
        }

        public BlobFileInfo ConvertToModel(BlobContainerClient containerClient, BlobItem blobItem)
        {
            return new BlobFileInfo
            {
                Name = blobItem.Name,
                Url = containerClient.Uri + "/" + blobItem.Name
            };
        }

        public async Task<bool> ExistsBlob(string rutaBlob)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

            var tmp2 = await containerClient.GetBlobClient(rutaBlob).ExistsAsync();

            return tmp2?.Value ?? false;
        }

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado)
        {
            // Cargar la imagen utilizando ImageSharp
            using (var imagenOriginal = Image.Load(rutaEntrada))
            {
                if (anchoDeseado > 0)
                {
                    // Realizar la operación de redimensionamiento
                    imagenOriginal.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(anchoDeseado, 0),
                        Mode = ResizeMode.Max
                    }));
                }

                // Convertir la imagen redimensionada a un flujo de memoria
                using (var memoryStream = new MemoryStream())
                {
                    imagenOriginal.SaveAsPng(memoryStream);
                    
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Obtener el contenedor
                    BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

                    // Obtener el blob
                    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

                    // Subir la imagen redimensionada al blob
                    await blobClient.UploadAsync(memoryStream, new BlobUploadOptions()
                    {
                        HttpHeaders = new BlobHttpHeaders()
                        {
                            ContentType = "image/png"
                        }
                    });
                }
            }
        }


        public async Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado)
        {
            // Cargar la imagen utilizando ImageSharp
            using (var imagenOriginal = Image.Load(rutaEntrada))
            {
                if (anchoDeseado > 0)
                {
                    // Realizar la operación de redimensionamiento
                    imagenOriginal.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(anchoDeseado, 0),
                        Mode = ResizeMode.Max
                    }));
                }

                // Convertir la imagen redimensionada a un flujo de memoria
                using (var memoryStream = new MemoryStream())
                {
                    imagenOriginal.SaveAsPng(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Obtener el contenedor
                    BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

                    // Obtener el blob
                    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

                    // Subir la imagen redimensionada al blob
                    await blobClient.UploadAsync(memoryStream, new BlobUploadOptions()
                    {
                        HttpHeaders = new BlobHttpHeaders()
                        {
                            ContentType = "image/png"
                        }
                    });
                }
            }
        }

        public async Task GuardarExcelEnAzureStorage(FileInfo archivoExcel, string blobLocation)
        {
            if (String.IsNullOrEmpty(blobLocation))
            {
                throw new ArgumentException("No se ha especificado la localización del blob a guardar");
            }
            if (archivoExcel.Extension.ToLower() != ".xlsx")
            {
                throw new ArgumentException("El archivo proporcionado no es un archivo Excel (.xlsx)");
            }

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

            BlobClient blobClient = containerClient.GetBlobClient($"{blobLocation}/{archivoExcel.Name}");

            // Abrir el archivo y cargarlo en un flujo de memoria
            using (var rutaEntrada = archivoExcel.OpenRead())
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copiar el archivo Excel al flujo de memoria
                    await rutaEntrada.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    await blobClient.UploadAsync(memoryStream, new BlobUploadOptions()
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        }
                    });
                }
            }
        }


        public async Task GuardarExcelEnAzureStorage(Stream stream, string blobLocation, string blobName)
        {
            // Crear un cliente de Blob para el contenedor
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(BlobStorageContainer);

            // Subir el archivo al contenedor
            BlobClient blobClient = containerClient.GetBlobClient($"{blobLocation}/{blobName}");
            await blobClient.UploadAsync(stream, overwrite: true);
        }


        public string CrearThumbnailName(string rutaImagen)
        {
            // Obtener el nombre del archivo sin la extensión
            string nombreArchivo = Path.GetFileNameWithoutExtension(rutaImagen);

            // Construir el nuevo nombre de archivo con el sufijo "_tn" y la extensión ".png"
            string nuevoNombreArchivo = $"{nombreArchivo}_tn.png";

            // Obtener la carpeta del archivo original
            string carpeta = Path.GetDirectoryName(rutaImagen);

            // Construir la nueva ruta completa para el thumbnail
            string nuevaRuta = Path.Combine(carpeta, nuevoNombreArchivo).Replace('\\', '/');

            return nuevaRuta;
        }
    }

}
