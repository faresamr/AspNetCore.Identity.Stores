using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class RolesTable<TRole, TKey> : TableStorage, IRolesTable<TRole, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "Role";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public RolesTable(IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
        }

        public async Task<IdentityResult> AddAsync(TRole role, CancellationToken cancellationToken)
        {
            return (await TableClient.UpsertEntityAsync(role.ToTableEntity(PartitionKey, ConvertToString(role.Id)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, ConvertToString(role.Id), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public IQueryable<TRole> Get()
        {
            Pageable<TableEntity> queryResultsFilter = TableClient.Query<TableEntity>(filter: PartitionFilter);
            return queryResultsFilter.Select(i => i.ConvertTo<TRole>()).AsQueryable();
        }

        public async Task<TRole> GetAsync(TKey roleId, CancellationToken cancellationToken)
        {
            var response = await TableClient.GetEntityAsync<TableEntity>(PartitionKey, ConvertToString(roleId), cancellationToken: cancellationToken);
            if (response.GetRawResponse().IsSuccess())
                return response.Value.ConvertTo<TRole>();
            else
                return null;
        }

        public async Task<TRole> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityRole<TKey>.NormalizedName)} eq '{normalizedName}'", cancellationToken: cancellationToken);
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return tableEntity.ConvertTo<TRole>();
            }

            return null;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            var tableEntity = role.ToTableEntity(PartitionKey, ConvertToString(role.Id));
            var response = await TableClient.UpsertEntityAsync(tableEntity, cancellationToken: cancellationToken);
            return response.ToIdentityResult();
        }
    }
}
