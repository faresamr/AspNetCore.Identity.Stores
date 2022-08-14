using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using Microsoft.Azure.Cosmos;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;

public static class IdentityStoresOptionsExtensions
{
    private static readonly Dictionary<IdentityStoresOptions, AzureCosmosDBOptions> options = new();

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string connectionString, string databaseId, string containerId = "AspNetIdentity")
    {
        options[identityStoresOptions] = new AzureCosmosDBOptions(connectionString, databaseId, containerId);
        CosmosClient cosmosClient = new(connectionString);
        var createDatabaseResponse = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
        _ = createDatabaseResponse.Database.CreateContainerIfNotExistsAsync(containerId, $"/{CosmosContainerEntity.PartitionKey}").Result;
        return identityStoresOptions;
    }

    public static string GetConnectionString(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].ConnectionString;
    public static string GetDatabaseId(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].DatabaseId;
    public static string GetContainerId(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].ContainerId;

    private record AzureCosmosDBOptions(string ConnectionString, string DatabaseId, string ContainerId);
}
