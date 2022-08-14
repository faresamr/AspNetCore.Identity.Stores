using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories;

internal abstract class TableStorage
{
    private readonly IDataProtector dataProtector;
    private readonly TableClient tableClient;

    protected TableStorage(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options)
    {
        dataProtector = dataProtectionProvider.CreateProtector(options.Value.GetTableName());
        tableClient = new(options.Value.GetConnectionString(), options.Value.GetTableName());
    }

    protected static string ConvertToString<T>(T value) where T : IEquatable<T> => Convert.ToString(value) ?? string.Empty;

    protected async Task<IdentityResult> AddAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken = default) where T : class
    {
        return (await tableClient.UpsertEntityAsync(entity.ToTableEntity(partitionKey, rowKey, dataProtector), cancellationToken: cancellationToken)).ToIdentityResult();
    }

    protected async Task<IdentityResult> DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
        return (await tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken)).ToIdentityResult();
    }
    protected async Task DeleteBulkAsync(string filter, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            throw new ArgumentException($"'{nameof(filter)}' cannot be null or whitespace.", nameof(filter));
        }

        AsyncPageable<TableEntity> queryResultsFilter = tableClient.QueryAsync<TableEntity>(filter: filter, cancellationToken: cancellationToken);
        await foreach (TableEntity tableEntity in queryResultsFilter)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await DeleteAsync(tableEntity.PartitionKey, tableEntity.RowKey, cancellationToken);
        }
    }

    protected async Task<IList<T>> QueryAsync<T>(string? filter = null, CancellationToken cancellationToken = default) where T : class, new()
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
    protected IEnumerable<T> Query<T>(string? filter = null, CancellationToken cancellationToken = default) where T : class, new()
    {
        Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(filter: filter, cancellationToken: cancellationToken);
        return queryResultsFilter.Select(i => i.ConvertTo<T>(dataProtector));
    }

    protected async Task<T?> QueryAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, new()
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

    protected async Task<IdentityResult> UpdateAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken = default) where T : class
    {
        var tableEntity = entity.ToTableEntity(partitionKey, rowKey, dataProtector);
        var response = await tableClient.UpsertEntityAsync(tableEntity, cancellationToken: cancellationToken);
        return response.ToIdentityResult();
    }
}
