using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IRoleClaimsTable<TRole, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IList<Claim>> GetAsync(TRole role, CancellationToken cancellationToken = default);
        Task DeleteRoleClaimsAsync(TRole role, CancellationToken cancellationToken);
    }

    public interface IRoleClaimsTable<TRole, TRoleClaim, TKey> : IRoleClaimsTable<TRole, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteAsync(TRoleClaim roleClaim, CancellationToken cancellationToken = default);
    }
}
