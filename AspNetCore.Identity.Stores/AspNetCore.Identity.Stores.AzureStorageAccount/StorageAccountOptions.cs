using AspNetCore.Identity.Stores.AzureStorageAccount.Repositories;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount
{
    public sealed class StorageAccountOptions
    {
        public string ConnectionString { get; private set; }

        public StorageAccountOptions UseAzureStorageAccount(string connectionString)
        {
            ConnectionString = connectionString;
            TableClient tableClient = new(connectionString, TableStorage.IdentityTable);
            tableClient.CreateIfNotExists();
            return this;
        }
    }
}
