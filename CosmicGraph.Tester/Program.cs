using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CosmicGraph.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var config = await LoadConfigAsync();

            var documentClient = CreateDocumentClient(config);
            var collection = await CreateCollectionAsync(documentClient, config);

            var client = new CosmicGraphClient(documentClient, collection);

            var example = new Example();
            await example.ExecuteAsync(client);
        }

        static async Task<Config> LoadConfigAsync()
        {
            return JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync("Config.json"));
        }

        static DocumentClient CreateDocumentClient(Config config)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return new DocumentClient(new Uri(config.Uri), config.AuthKey, serializerSettings);
        }

        static async Task<DocumentCollection> CreateCollectionAsync(DocumentClient documentClient, Config config)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                config.DatabaseId,
                config.CollectionId
            );

            await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = config.DatabaseId });

            return await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(config.DatabaseId), new DocumentCollection { Id = config.CollectionId });
        }
    }
}
