using AspNetCore.Identity.Stores.AzureStorageAccount.Repositories;
using Azure.Data.Tables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions
{
    public static class IdentityStoresOptionsExtensions
    {
        private static readonly Hashtable connectionStrings = new();

        public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, string connectionString, string tableName = "AspNetIdentity")
        {
            connectionStrings[identityStoresOptions] = new AzureStorageAccountOptions(connectionString, tableName);
            TableClient tableClient = new(connectionString, tableName);
            tableClient.CreateIfNotExists();
            return identityStoresOptions;
        }

        public static string GetConnectionString(this IdentityStoresOptions identityStoresOptions) => (connectionStrings[identityStoresOptions] as AzureStorageAccountOptions).ConnectionString;
        public static string GetTableName(this IdentityStoresOptions identityStoresOptions) => (connectionStrings[identityStoresOptions] as AzureStorageAccountOptions).TableName;
    
        private record AzureStorageAccountOptions(string ConnectionString, string TableName);
    }
}
