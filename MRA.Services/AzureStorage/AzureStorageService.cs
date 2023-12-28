using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string BlobStorageContainer;
        public readonly string BlobURL;

        public AzureStorageService(string connectionString, string blobStorageContainer, string blobUrl)
        {
            BlobStorageContainer = blobStorageContainer;
            _blobServiceClient = new BlobServiceClient(connectionString);
            BlobURL = blobUrl;
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

        public string CrearThumbnailName(string rutaImagen)
        {
            // Obtener el nombre del archivo sin la extensión
            string nombreArchivo = Path.GetFileNameWithoutExtension(rutaImagen);

            // Construir el nuevo nombre de archivo con el sufijo "_tn" y la extensión ".png"
            string nuevoNombreArchivo = $"{nombreArchivo}_tn.png";

            // Obtener la carpeta del archivo original
            string carpeta = Path.GetDirectoryName(rutaImagen);

            // Construir la nueva ruta completa para el thumbnail
            string nuevaRuta = Path.Combine(carpeta, nuevoNombreArchivo);

            return nuevaRuta;
        }
    }

}
