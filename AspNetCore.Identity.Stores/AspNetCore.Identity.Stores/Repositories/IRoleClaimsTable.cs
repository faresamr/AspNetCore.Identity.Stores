using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IRoleClaimsTable<TRole, TKey>
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IList<Claim>> GetAsync(TRole role, CancellationToken cancellationToken = default);
    Task DeleteRoleClaimsAsync(TRole role, CancellationToken cancellationToken = default);
}

public interface IRoleClaimsTable<TRole, TRoleClaim, TKey> : IRoleClaimsTable<TRole, TKey>
    where TRole : IdentityRole<TKey>, new()
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default);
}
