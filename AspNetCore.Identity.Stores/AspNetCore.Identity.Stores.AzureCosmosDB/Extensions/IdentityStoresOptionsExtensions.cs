using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Extensions
{
    public static class IdentityStoresOptionsExtensions
    {
        private static readonly Hashtable options = new();

        public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string connectionString, string databaseId, string containerId = "AspNetIdentity")
        {
            options[identityStoresOptions] = new AzureCosmosDBOptions(connectionString, databaseId, containerId);
            CosmosClient cosmosClient = new(connectionString);
            var createDatabaseResponse = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            _ = createDatabaseResponse.Database.CreateContainerIfNotExistsAsync(containerId, $"/{TableEntity.PartitionKey}").Result;
            return identityStoresOptions;
        }

        public static string GetConnectionString(this IdentityStoresOptions identityStoresOptions) => (options[identityStoresOptions] as AzureCosmosDBOptions).ConnectionString;
        public static string GetDatabaseId(this IdentityStoresOptions identityStoresOptions) => (options[identityStoresOptions] as AzureCosmosDBOptions).DatabaseId;
        public static string GetContainerId(this IdentityStoresOptions identityStoresOptions) => (options[identityStoresOptions] as AzureCosmosDBOptions).ContainerId;

        private record AzureCosmosDBOptions(string ConnectionString, string DatabaseId, string ContainerId);
    }
}
