namespace MRA.Infrastructure.Storage;

public interface IStorageProvider
{
    Task<List<BlobFileInfo>> ListBlobFilesAsync();

    Task<bool> ExistsBlob(string rutaBlob);

    Task<bool> ResizeAndSave(MemoryStream inputStream, string blobName, int width);
    Task<bool> ResizeAndSave(string path, string blobName, int width);

    Task<bool> Save(Stream stream, string blobLocation, string blobName);

    string CrearThumbnailName(string imagePath);

    string GetBlobURL();
}
