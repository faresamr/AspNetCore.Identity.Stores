using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class RoleClaimsTable<TRole, TRoleClaim, TKey> : TableStorage, IRoleClaimsTable<TRole, TRoleClaim, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "RoleClaim";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public RoleClaimsTable(IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
        }

        public async Task<IdentityResult> AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default)
        {
            return (await TableClient.UpsertEntityAsync(roleClaim.ToTableEntity(PartitionKey, GetHashKey(roleClaim)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, GetHashKey(roleClaim), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IList<Claim>> GetAsync(TRole role, CancellationToken cancellationToken = default)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityRoleClaim<TKey>.RoleId)} eq '{role.Id}'", cancellationToken: cancellationToken);
            List<Claim> claims = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                claims.Add(tableEntity.ConvertTo<TRoleClaim>().ToClaim());
            }
            return claims;
        }

        private static string GetHashKey(TRoleClaim roleClaim) => $"{roleClaim.RoleId}-{roleClaim.ClaimType}-{roleClaim.ClaimValue}".GetHashString();
    }
}
