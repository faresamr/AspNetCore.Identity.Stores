﻿using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories;

internal class RoleClaimsTable<TRole, TRoleClaim, TKey> : TableStorage, IRoleClaimsTable<TRole, TRoleClaim, TKey>
    where TRole : IdentityRole<TKey>, new()
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string PartitionKey = "RoleClaim";

    public RoleClaimsTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
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
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and RoleId eq {role.Id}");
        return (await QueryAsync<TRoleClaim>(filter: filter, cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
    }

    public Task DeleteRoleClaimsAsync(TRole role, CancellationToken cancellationToken)
    {
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and RoleId eq {role.Id}");
        return DeleteBulkAsync(filter, cancellationToken);
    }

    private static string GetHashKey(TRoleClaim roleClaim) => $"{roleClaim.RoleId}-{roleClaim.ClaimType}-{roleClaim.ClaimValue}".GetHashString();
}
