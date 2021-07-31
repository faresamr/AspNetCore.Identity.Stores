using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public RoleClaimsTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStorageAccountOptions> options) : base(dataProtectionProvider, options, IdentityTable)
        {
        }

        public Task<IdentityResult> AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default)
        {
            return AddAsync(PartitionKey, GetHashKey(roleClaim), roleClaim, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(PartitionKey, GetHashKey(roleClaim), cancellationToken: cancellationToken);
        }

        public async Task<IList<Claim>> GetAsync(TRole role, CancellationToken cancellationToken = default)
        {
            return (await QueryAsync<TRoleClaim>(filter: $"{PartitionFilter} and {nameof(IdentityRoleClaim<TKey>.RoleId)} eq '{role.Id}'", cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
        }

        private static string GetHashKey(TRoleClaim roleClaim) => $"{roleClaim.RoleId}-{roleClaim.ClaimType}-{roleClaim.ClaimValue}".GetHashString();
    }
}
