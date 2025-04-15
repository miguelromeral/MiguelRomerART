namespace MRA.Infrastructure.Storage;

public interface IStorageProvider
{
    Task<List<BlobFileInfo>> ListBlobFilesAsync();

    Task<bool> ExistsBlob(string rutaBlob);

    Task ResizeAndSave(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado);

    Task Save(Stream stream, string blobLocation, string blobName);

    string CrearThumbnailName(string rutaImagen);

    string GetBlobURL();
}
