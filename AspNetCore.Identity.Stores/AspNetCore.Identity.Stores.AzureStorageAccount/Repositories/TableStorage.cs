﻿using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector dataProtector;
        private readonly TableClient tableClient;

        public TableStorage(IDataProtectionProvider dataProtectionProvider, IOptions<StorageAccountOptions> options, string tableName)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
            }

            dataProtector = dataProtectionProvider.CreateProtector(tableName);
            tableClient = new(options.Value.ConnectionString, tableName);
        }
    
        protected static string ConvertToString<T>(T value) where T : IEquatable<T> => Convert.ToString(value);

        protected async Task<IdentityResult> AddAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken = default) where T : class
        {
            return (await tableClient.UpsertEntityAsync(entity.ToTableEntity(partitionKey, rowKey, dataProtector), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        protected async Task<IdentityResult> DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
        {
            return (await tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken)).ToIdentityResult();
        }

        protected async Task<IList<T>> QueryAsync<T>(string filter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            AsyncPageable<TableEntity> queryResultsFilter = tableClient.QueryAsync<TableEntity>(filter: filter, cancellationToken: cancellationToken);
            List<T> entities = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                entities.Add(tableEntity.ConvertTo<T>(dataProtector));
            }
            return entities;
        }
        protected IEnumerable<T> Query<T>(string filter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(filter: filter, cancellationToken: cancellationToken);
            return queryResultsFilter.Select(i => i.ConvertTo<T>(dataProtector));
        }

        protected async Task<T> QueryAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, new()
        {
            try
            {
                var response = await tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
                if (response.GetRawResponse().IsSuccess())
                    return response.Value.ConvertTo<T>(dataProtector);
                else
                    return null;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<IdentityResult> UpdateAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken) where T : class
        {
            var tableEntity = entity.ToTableEntity(partitionKey, rowKey, dataProtector);
            var response = await tableClient.UpsertEntityAsync(tableEntity, cancellationToken: cancellationToken);
            return response.ToIdentityResult();
        }
    }
}