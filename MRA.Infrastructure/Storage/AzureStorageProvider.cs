using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MRA.Infrastructure.Settings;
using SixLabors.ImageSharp;
using MRA.Infrastructure.Storage.Connection;

namespace MRA.Infrastructure.Storage;

public class AzureStorageProvider : IStorageProvider
{
    private readonly string blobContainer;
    private readonly string blobPath;
    private readonly IAzureStorageConnection _connection;


    public AzureStorageProvider(AppSettings config, IAzureStorageConnection connection)
    {
        blobContainer = config.AzureStorage.BlobStorageContainer;
        blobPath = config.AzureStorage.BlobPath;
        _connection = connection;
    }

    public async Task<List<BlobFileInfo>> ListBlobFilesAsync()
    {
        var containerClient = _connection.GetContainer(blobContainer);
        return await _connection.GetListBlobsAsync(blobContainer).Select(model => ConvertToModel(containerClient, model)).ToListAsync();
    }

    public string GetBlobURL() => blobPath;

    private BlobFileInfo ConvertToModel(BlobContainerClient containerClient, BlobItem blobItem)
    {
        return new BlobFileInfo
        {
            Name = blobItem.Name,
            Url = $"{containerClient.Uri}/{blobItem.Name}"
        };
    }

    public async Task<bool> ExistsBlob(string rutaBlob)
    {
        return await _connection.BlobExists(blobContainer, rutaBlob);
    }

    public async Task<bool> ResizeAndSave(string path, string blobName, int width)
    {
        using var imagenOriginal = await path.LoadImageAsync();
        imagenOriginal.ResizeImageIfNecessary(width);
        return await UploadImageToBlob(blobName, imagenOriginal);
    }

    public async Task<bool> ResizeAndSave(MemoryStream inputStream, string blobName, int width)
    {
        using var imagenOriginal = await inputStream.LoadImageAsync();
        imagenOriginal.ResizeImageIfNecessary(width);
        return await UploadImageToBlob(blobName, imagenOriginal);
    }

    private async Task<bool> UploadImageToBlob(string blobPath, Image image)
    {
        using var memoryStream = new MemoryStream();
        await image.SaveAsPngAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return await _connection.UploadImageAsync(blobContainer, blobPath, memoryStream) is not null;
    }

    public async Task<bool> Save(Stream stream, string blobLocation, string blobName)
    {
        return await _connection.UploadAsync(blobContainer, blobName, stream) is not null;
    }

    public string CrearThumbnailName(string imagePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(imagePath);
        string thumbnailFileName = $"{fileName}_tn.png";

        string folder = Path.GetDirectoryName(imagePath) ?? string.Empty;
        string newPath = Path.Combine(folder, thumbnailFileName).Replace('\\', '/');

        return newPath;
    }
}
