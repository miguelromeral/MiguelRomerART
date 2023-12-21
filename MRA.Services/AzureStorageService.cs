using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace MRA.Services
{
    public class AzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly string blobStorageContainer;
        public readonly string BlobURL;

        public AzureStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = configuration.GetValue<string>("AzureStorage:ConnectionString");
            blobStorageContainer = configuration.GetValue<string>("AzureStorage:BlobStorageContainer");
            _blobServiceClient = new BlobServiceClient(connectionString);
            BlobURL = configuration.GetValue<string>("AzureStorage:BlobPath");
        }

        public async Task<List<BlobFileInfo>> ListBlobFilesAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobStorageContainer);

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
    }

}
