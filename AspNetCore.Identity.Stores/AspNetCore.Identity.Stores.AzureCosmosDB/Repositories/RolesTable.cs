using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;

internal class RolesTable<TRole, TKey> : CosmosContainer, IRolesTable<TRole, TKey>
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string PartitionKey = "Role";
    private readonly IRoleClaimsTable<TRole, TKey> roleClaimsTable;
    private readonly IUserRolesTable<TKey> userRolesTable;

    public RolesTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options, IRoleClaimsTable<TRole, TKey> roleClaimsTable, IUserRolesTable<TKey> userRolesTable) : base(dataProtectionProvider, options)
    {
        this.roleClaimsTable = roleClaimsTable;
        this.userRolesTable = userRolesTable;
    }

    public Task<IdentityResult> AddAsync(TRole role, CancellationToken cancellationToken)
    {
        return AddAsync(PartitionKey, ConvertToString(role.Id), role, cancellationToken: cancellationToken);
    }

    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        await Task.WhenAll(new[]
        {
            roleClaimsTable.DeleteRoleClaimsAsync(role, cancellationToken),
            userRolesTable.DeleteRoleUsersAsync(role.Id, cancellationToken)
        });
        return await DeleteAsync(PartitionKey, ConvertToString(role.Id), cancellationToken: cancellationToken);
    }

    public IQueryable<TRole> Get() => Query<TRole>(BuildQuery(PartitionKey)).AsQueryable();

    public Task<TRole?> GetAsync(TKey roleId, CancellationToken cancellationToken)
    {
        return QueryAsync<TRole>(PartitionKey, ConvertToString(roleId), cancellationToken: cancellationToken);
    }
    public async Task<IEnumerable<TRole>> GetAsync(CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey);
        return await QueryAsync<TRole>(qry, cancellationToken: cancellationToken);
    }

    public async Task<TRole?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey, (nameof(IdentityRole<TKey>.NormalizedName), normalizedName));
        return (await QueryAsync<TRole>(qry, cancellationToken: cancellationToken)).FirstOrDefault();
    }

    public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        return UpdateAsync(PartitionKey, ConvertToString(role.Id), role, cancellationToken);
    }
}
