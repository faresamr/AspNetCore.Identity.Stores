using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserRolesTable<TUserRole, TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TUserRole userRole, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TKey userId, TKey roleId, CancellationToken cancellationToken);
        Task<IList<TUserRole>> GetUsersAsync(TKey roleId, CancellationToken cancellationToken);
        Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken);
    }
}
