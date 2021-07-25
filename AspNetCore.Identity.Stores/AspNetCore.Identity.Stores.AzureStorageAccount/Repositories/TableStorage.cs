using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal abstract class TableStorage
    {
        internal const string IdentityTable = "Identity";

        public TableStorage(IOptions<StorageAccountOptions> options, string tableName)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
            }

            TableClient = new(options.Value.ConnectionString, tableName);
        }
    
        protected TableClient TableClient { get; }

        protected static string ConvertToString<T>(T value) where T : IEquatable<T> => Convert.ToString(value);
    }
}
