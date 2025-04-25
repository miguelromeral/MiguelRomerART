namespace MRA.Services.Storage;

public interface IStorageService
{
    Task<bool> ExistsBlob(string rutaBlob);

    Task<bool> ResizeAndSave(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado);

    Task<bool> Save(Stream stream, string blobLocation, string blobName);

    string CrearThumbnailName(string rutaImagen);

    string GetBlobURL();
}
