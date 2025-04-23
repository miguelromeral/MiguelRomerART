using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MRA.Infrastructure.Storage.Connection;

public interface IAzureStorageConnection
{
    BlobContainerClient GetContainer(string containerName);


    AsyncPageable<BlobItem> GetListBlobsAsync(string containerName);

    Task<bool> BlobExists(string containerName, string blobPath);

    BlobClient GetBlob(string containerName, string blobPath);

    Task<BlobContentInfo> UploadAsync(string containerName, string blobPath, Stream stream);
    Task<BlobContentInfo> UploadImageAsync(string containerName, string blobPath, MemoryStream memoryStream);
}
