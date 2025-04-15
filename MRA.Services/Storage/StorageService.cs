using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Storage;

namespace MRA.Services.Storage;

public class StorageService : IStorageService
{
    private readonly IStorageDatabase _database;
    private readonly AppSettings _config;

    public StorageService(AppSettings config, IStorageDatabase db)
    {
        _config = config;
        _database = db;
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
        return await _database.ExistsBlob(rutaBlob);
    }

    public async Task ResizeAndSave(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado)
    {
        await _database.ResizeAndSave(rutaEntrada, nombreBlob, anchoDeseado);
    }

    public async Task Save(Stream stream, string blobLocation, string blobName)
    {
        await _database.Save(stream, blobLocation, blobName);
    }

    public string CrearThumbnailName(string rutaImagen)
    {
        return _database.CrearThumbnailName(rutaImagen);
    }

    public string GetBlobURL() => _database.GetBlobURL();
}
