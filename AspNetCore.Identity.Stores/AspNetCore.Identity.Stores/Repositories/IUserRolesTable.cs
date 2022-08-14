using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IUserRolesTable<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> DeleteAsync(TKey userId, TKey roleId, CancellationToken cancellationToken = default);
    Task DeleteUserRolesAsync(TKey userId, CancellationToken cancellationToken = default);
    Task DeleteRoleUsersAsync(TKey roleId, CancellationToken cancellationToken = default);
}
public interface IUserRolesTable<TUserRole, TKey> : IUserRolesTable<TKey>
    where TUserRole : IdentityUserRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TUserRole userRole, CancellationToken cancellationToken = default);
    Task<IList<TUserRole>> GetUsersAsync(TKey roleId, CancellationToken cancellationToken = default);
    Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken = default);
}
