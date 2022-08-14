using Azure.Data.Tables;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;

public static class IdentityStoresOptionsExtensions
{
    private static readonly Dictionary<IdentityStoresOptions, AzureStorageAccountOptions> options = new();

    public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, string connectionString, string tableName = "AspNetIdentity")
    {
        options[identityStoresOptions] = new AzureStorageAccountOptions(connectionString, tableName);
        TableClient tableClient = new(connectionString, tableName);
        tableClient.CreateIfNotExists();
        return identityStoresOptions;
    }

    public static string GetConnectionString(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].ConnectionString;
    public static string GetTableName(this IdentityStoresOptions identityStoresOptions) => options[identityStoresOptions].TableName;

    private record AzureStorageAccountOptions(string ConnectionString, string TableName);
}
