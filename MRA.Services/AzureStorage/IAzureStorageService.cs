using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MRA.DTO.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.AzureStorage;

public interface IAzureStorageService
{
    Task<List<BlobFileInfo>> ListBlobFilesAsync();

    BlobFileInfo ConvertToModel(BlobContainerClient containerClient, BlobItem blobItem);

    Task<bool> ExistsBlob(string rutaBlob);

    Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado);

    Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado);

    Task GuardarExcelEnAzureStorage(FileInfo archivoExcel, string blobLocation);

    Task GuardarExcelEnAzureStorage(Stream stream, string blobLocation, string blobName);

    string CrearThumbnailName(string rutaImagen);

    string GetBlobURL();
}
