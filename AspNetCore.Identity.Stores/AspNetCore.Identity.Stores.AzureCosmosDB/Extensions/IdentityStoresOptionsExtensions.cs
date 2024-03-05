using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;

public static class IdentityStoresOptionsExtensions
{
    private static readonly Dictionary<IdentityStoresOptions, AzureCosmosDBOptions> options = [];

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string connectionString, string databaseId, string containerId = "AspNetIdentity", CosmosClientOptions? clientOptions = null) => identityStoresOptions.UseAzureCosmosDB(new CosmosClient(connectionString, clientOptions), databaseId, containerId);

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string accountEndpoint, string authKeyOrResourceToken, string databaseId, string containerId = "AspNetIdentity", CosmosClientOptions? clientOptions = null) => identityStoresOptions.UseAzureCosmosDB(new CosmosClient(accountEndpoint, authKeyOrResourceToken, clientOptions), databaseId, containerId);

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string accountEndpoint, AzureKeyCredential authKeyOrResourceTokenCredential, string databaseId, string containerId = "AspNetIdentity", CosmosClientOptions? clientOptions = null) => identityStoresOptions.UseAzureCosmosDB(new CosmosClient(accountEndpoint, authKeyOrResourceTokenCredential, clientOptions), databaseId, containerId);

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, string accountEndpoint, TokenCredential tokenCredential, string databaseId, string containerId = "AspNetIdentity", CosmosClientOptions? clientOptions = null) => identityStoresOptions.UseAzureCosmosDB(new CosmosClient(accountEndpoint, tokenCredential, clientOptions), databaseId, containerId);

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, Func<CosmosClient> clientFactory, string databaseId, string containerId = "AspNetIdentity") => identityStoresOptions.UseAzureCosmosDB(clientFactory.Invoke(), databaseId, containerId);

    public static IdentityStoresOptions UseAzureCosmosDB(this IdentityStoresOptions identityStoresOptions, CosmosClient cosmosClient, string databaseId, string containerId = "AspNetIdentity")
    {
        AzureCosmosDBOptions azureCosmosDBOptions = new(cosmosClient, databaseId, containerId);
        options[identityStoresOptions] = azureCosmosDBOptions;

        return identityStoresOptions;
    }

    public static CosmosClient GetCosmosClient(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].CosmosClient;
    public static string GetDatabaseId(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].DatabaseId;
    public static string GetContainerId(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].ContainerId;

    private record AzureCosmosDBOptions(CosmosClient CosmosClient, string DatabaseId, string ContainerId);
}
