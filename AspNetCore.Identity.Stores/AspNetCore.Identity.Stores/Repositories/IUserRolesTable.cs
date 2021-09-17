using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserRolesTable<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> DeleteAsync(TKey userId, TKey roleId, CancellationToken cancellationToken);
        Task DeleteUserRolesAsync(TKey userId, CancellationToken cancellationToken);
        Task DeleteRoleUsersAsync(TKey roleId, CancellationToken cancellationToken);
    }
    public interface IUserRolesTable<TUserRole, TKey> : IUserRolesTable<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TUserRole userRole, CancellationToken cancellationToken);
        Task<IList<TUserRole>> GetUsersAsync(TKey roleId, CancellationToken cancellationToken);
        Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken);
    }
}
