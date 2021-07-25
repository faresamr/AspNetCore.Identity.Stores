using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class UserRolesTable<TUserRole, TKey> : TableStorage, IUserRolesTable<TUserRole, TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserRole";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public UserRolesTable(IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
        }

        public async Task<IdentityResult> CreateAsync(TUserRole userToken, CancellationToken cancellationToken)
        {
            return (await TableClient.UpsertEntityAsync(userToken.ToTableEntity(PartitionKey, GetHashKey(userToken)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, GetHashKey(userId, roleId), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IList<TUserRole>> GetUsersAsync(TKey roleId, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserRole<TKey>.RoleId)} eq '{roleId}'", cancellationToken: cancellationToken);
            List<TUserRole> userRoles = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                userRoles.Add(tableEntity.ConvertTo<TUserRole>());
            }
            return userRoles;
        }

        public async Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserRole<TKey>.UserId)} eq '{userId}'", cancellationToken: cancellationToken);
            List<TUserRole> userRoles = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                userRoles.Add(tableEntity.ConvertTo<TUserRole>());
            }
            return userRoles;
        }

        private static string GetHashKey(TUserRole userRole) => GetHashKey(userRole.UserId, userRole.RoleId);
        private static string GetHashKey(TKey userId, TKey roleId) => $"{userId}-{roleId}".GetHashString();
    }
}
