using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class UsersTable<TUser, TKey> : TableStorage, IUsersTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "User";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";
        public UsersTable(IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            return (await TableClient.UpsertEntityAsync(user.ToTableEntity(PartitionKey, ConvertToString(user.Id)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, ConvertToString(user.Id), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public IQueryable<TUser> Get()
        {
            Pageable<TableEntity> queryResultsFilter = TableClient.Query<TableEntity>(filter: PartitionFilter);
            return queryResultsFilter.Select(i => i.ConvertTo<TUser>()).AsQueryable();
        }

        public async Task<TUser> GetAsync(TKey userId, CancellationToken cancellationToken)
        {
            var response = await TableClient.GetEntityAsync<TableEntity>(PartitionKey, ConvertToString(userId), cancellationToken: cancellationToken);
            if (response.GetRawResponse().IsSuccess())
                return response.Value.ConvertTo<TUser>();
            else
                return null;
        }

        public async Task<TUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUser<TKey>.NormalizedEmail)} eq '{normalizedEmail}'", cancellationToken: cancellationToken);
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return tableEntity.ConvertTo<TUser>();
            }

            return null;
        }

        public async Task<TUser> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUser<TKey>.NormalizedUserName)} eq '{normalizedUserName}'", cancellationToken: cancellationToken);
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return tableEntity.ConvertTo<TUser>();
            }

            return null;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var tableEntity = user.ToTableEntity(PartitionKey, ConvertToString(user.Id));
            var response = await TableClient.UpsertEntityAsync(tableEntity, cancellationToken: cancellationToken);
            return response.ToIdentityResult();
        }
    }
}
