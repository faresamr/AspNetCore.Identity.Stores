using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;

internal class RoleClaimsTable<TRole, TRoleClaim, TKey> : CosmosContainer, IRoleClaimsTable<TRole, TRoleClaim, TKey>
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
        var qry = BuildQuery(PartitionKey, (nameof(IdentityRoleClaim<TKey>.RoleId), role.Id));
        return (await QueryAsync<TRoleClaim>(qry, cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
    }

    private static string GetHashKey(TRoleClaim roleClaim) => $"{roleClaim.RoleId}-{roleClaim.ClaimType}-{roleClaim.ClaimValue}".GetHashString();

    public Task DeleteRoleClaimsAsync(TRole role, CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey, (nameof(IdentityRoleClaim<TKey>.RoleId), role.Id));
        return DeleteBulkAsync(qry, cancellationToken);
    }
}
