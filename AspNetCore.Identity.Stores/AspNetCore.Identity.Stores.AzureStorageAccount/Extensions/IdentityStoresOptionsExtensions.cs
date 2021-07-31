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

        public static IdentityStoresOptions UseAzureStorageAccount(this IdentityStoresOptions identityStoresOptions, string connectionString)
        {
            connectionStrings[identityStoresOptions] = connectionString;
            TableClient tableClient = new(connectionString, TableStorage.IdentityTable);
            tableClient.CreateIfNotExists();
            return identityStoresOptions;
        }

        public static string GetConnectionString(this IdentityStoresOptions identityStoresOptions) => connectionStrings[identityStoresOptions] as string;
    }
}
