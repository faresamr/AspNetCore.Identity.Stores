using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IUsersTable<TUser, TKey>
    where TUser : IdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TUser user, CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default);
    Task<TUser?> GetAsync(TKey userId, CancellationToken cancellationToken = default);
    Task<TUser?> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = default);
    Task<TUser?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    IQueryable<TUser> Get();
}
