using Microsoft.Azure.Cosmos;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Infrastructure.Database.Providers
{
    public class AzureCosmosDbDatabase : IDocumentsDatabase
    {
        private readonly AppSettings _appConfiguration;

        private CosmosClient _cosmosClient;
        private CosmosClient CosmosClient
        {
            get
            {
                if (_cosmosClient == null)
                    InitializeCosmosDb();

                return _cosmosClient;
            }
        }

        public AzureCosmosDbDatabase(AppSettings appConfig)
        {
            _appConfiguration = appConfig;
        }

        private void InitializeCosmosDb()
        {
            CosmosClientOptions options =
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    RequestTimeout = TimeSpan.FromSeconds(_appConfiguration.AzureCosmosDb.TimeoutSeconds),
                };

            _cosmosClient = new CosmosClient(_appConfiguration.AzureCosmosDb.ConnectionString, options);
        }

        public async Task<IEnumerable<IDocument>> GetAllDocumentsAsync<IDocument>(string collection)
        {
            var query = CosmosClient.GetContainer(_appConfiguration.AzureCosmosDb.DatabaseName, collection).GetItemQueryIterator<IDocument>();

            var results = new List<IDocument>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IDocument> GetDocumentAsync<IDocument>(string collection, string documentId)
        {
            try
            {
                var response = await CosmosClient.GetContainer(_appConfiguration.AzureCosmosDb.DatabaseName, collection).ReadItemAsync<IDocument>(documentId, new PartitionKey(documentId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<bool> DocumentExistsAsync(string collection, string documentId)
        {
            var document = await GetDocumentAsync<IDocument>(collection, documentId);
            return document != null;
        }

        public async Task<bool> SetDocumentAsync(string collection, string documentId, IDocument document)
        {
            var response = await CosmosClient.GetContainer(_appConfiguration.AzureCosmosDb.DatabaseName, collection).UpsertItemAsync(document, new PartitionKey(documentId));
            return response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<bool> DeleteDocumentAsync(string collection, string id)
        {
            try
            {
                var response = await CosmosClient.GetContainer(_appConfiguration.AzureCosmosDb.DatabaseName, collection).DeleteItemAsync<IDocument>(id, new PartitionKey(id));
                return response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
