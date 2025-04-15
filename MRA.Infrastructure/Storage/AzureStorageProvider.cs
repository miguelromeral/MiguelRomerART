using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MRA.Infrastructure.Settings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MRA.Infrastructure.Storage;

public class AzureStorageProvider : IStorageProvider
{
    private readonly string blobContainer;
    private readonly string connectionString;
    private readonly string blobPath;

    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageProvider(AppSettings config)
    {
        blobContainer = config.AzureStorage.BlobStorageContainer;
        connectionString = config.AzureStorage.ConnectionString;
        blobPath = config.AzureStorage.BlobPath;
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<List<BlobFileInfo>> ListBlobFilesAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainer);

        var blobFiles = new List<BlobFileInfo>();
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            blobFiles.Add(ConvertToModel(containerClient, blobItem));
        }

        return blobFiles;
    }

    public string GetBlobURL() => blobPath;

    private BlobFileInfo ConvertToModel(BlobContainerClient containerClient, BlobItem blobItem)
    {
        return new BlobFileInfo
        {
            Name = blobItem.Name,
            Url = containerClient.Uri + "/" + blobItem.Name
        };
    }

    public async Task<bool> ExistsBlob(string rutaBlob)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainer);

        var tmp2 = await containerClient.GetBlobClient(rutaBlob).ExistsAsync();

        return tmp2?.Value ?? false;
    }

    public async Task ResizeAndSave(string rutaEntrada, string nombreBlob, int anchoDeseado)
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
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainer);

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


    public async Task ResizeAndSave(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado)
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
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainer);

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

    public async Task Save(Stream stream, string blobLocation, string blobName)
    {
        BlobClientOptions options = new BlobClientOptions
        {
            Diagnostics = { IsLoggingEnabled = true, IsTelemetryEnabled = true }
        };
        var blobServiceClient = new BlobServiceClient(connectionString, options);
        var containerClient = blobServiceClient.GetBlobContainerClient(blobContainer);

        BlobClient blobClient = containerClient.GetBlobClient($"{blobLocation}/{blobName}");
        await blobClient.UploadAsync(stream, overwrite: true);
    }


    public string CrearThumbnailName(string rutaImagen)
    {
        string nombreArchivo = Path.GetFileNameWithoutExtension(rutaImagen);

        string nuevoNombreArchivo = $"{nombreArchivo}_tn.png";

        string carpeta = Path.GetDirectoryName(rutaImagen);

        string nuevaRuta = Path.Combine(carpeta, nuevoNombreArchivo).Replace('\\', '/');

        return nuevaRuta;
    }
}
