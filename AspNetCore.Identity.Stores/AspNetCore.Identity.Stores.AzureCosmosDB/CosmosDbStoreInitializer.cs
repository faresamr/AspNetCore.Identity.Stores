
using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Stores.AzureCosmosDB;

internal class CosmosDbStoreInitializer(IOptions<IdentityStoresOptions> options) : IStoreInitializer
{
    public async Task InitializeAsync()
    {
        IdentityStoresOptions value = options.Value;
        CosmosClient client = value.GetCosmosClient();

        var createDatabaseResponse = await client.CreateDatabaseIfNotExistsAsync(value.GetDatabaseId());
        _ = await createDatabaseResponse.Database.CreateContainerIfNotExistsAsync(value.GetContainerId(), $"/{CosmosContainerEntity.PartitionKey}");
    }
}