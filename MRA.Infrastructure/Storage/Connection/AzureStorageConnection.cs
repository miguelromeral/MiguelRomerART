using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MRA.Infrastructure.Settings;
using SharpCompress.Compressors.Xz;
using System.Data.Common;
using System.IO;

namespace MRA.Infrastructure.Storage.Connection;

public class AzureStorageConnection : IAzureStorageConnection
{
    private readonly string _connectionString;
    private BlobServiceClient? _blobServiceClient;
    private BlobServiceClient BlobServiceClient
    {
        get
        {
            if (_blobServiceClient is null)
                _blobServiceClient = new BlobServiceClient(_connectionString);

            return _blobServiceClient;
        }
    }

    public AzureStorageConnection(AppSettings config)
    {
        _connectionString = config.AzureStorage.ConnectionString;
    }

    public BlobContainerClient GetContainer(string containerName) => BlobServiceClient.GetBlobContainerClient(containerName);

    public AsyncPageable<BlobItem> GetListBlobsAsync(string containerName) => GetContainer(containerName).GetBlobsAsync();

    public BlobClient GetBlob(string containerName, string blobPath) => GetContainer(containerName).GetBlobClient(blobPath);

    public async Task<bool> BlobExists(string containerName, string blobPath) => await GetBlob(containerName, blobPath).ExistsAsync();

    public async Task<BlobContentInfo> UploadAsync(string containerName, string blobPath, Stream stream) 
        => await GetBlob(containerName, blobPath).UploadAsync(stream, overwrite: true);

    public async Task<BlobContentInfo> UploadImageAsync(string containerName, string blobPath, MemoryStream memoryStream)
    {
        return await GetBlob(containerName, blobPath).UploadAsync(memoryStream, new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders()
            {
                ContentType = "image/png"
            }
        });
    }
}
