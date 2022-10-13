using Azure;
using Azure.Core;
using Azure.Data.Tables;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;

public static class IdentityStoresOptionsExtensions
{
    private static readonly Dictionary<IdentityStoresOptions, TableClient> options = new();

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Uri endpoint, TableClientOptions? options = null) => identityStoresOptions.UseAzureStorageAccount(new TableClient(endpoint, options));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Uri endpoint, AzureSasCredential credential, TableClientOptions? options = null) => identityStoresOptions.UseAzureStorageAccount(new TableClient(endpoint, credential, options));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Uri endpoint, string tableName, TableSharedKeyCredential credential) => identityStoresOptions.UseAzureStorageAccount(new TableClient(endpoint, tableName, credential));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Uri endpoint, string tableName, TableSharedKeyCredential credential, TableClientOptions? options = null) => identityStoresOptions.UseAzureStorageAccount(new TableClient(endpoint, tableName, credential, options));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, string connectionString, string tableName = "AspNetIdentity", TableClientOptions? options = null) => identityStoresOptions.UseAzureStorageAccount(new TableClient(connectionString, tableName, options));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Uri endpoint, string tableName, TokenCredential tokenCredential, TableClientOptions? options = default) => identityStoresOptions.UseAzureStorageAccount(new TableClient(endpoint, tableName, tokenCredential, options));

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, Func<TableClient> factory) => identityStoresOptions.UseAzureStorageAccount(factory.Invoke());

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, TableClient tableClient)
    {
        tableClient.CreateIfNotExists();
        options[identityStoresOptions] = tableClient;
        return identityStoresOptions;
    }


    public static TableClient GetTableClient(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions];
}
