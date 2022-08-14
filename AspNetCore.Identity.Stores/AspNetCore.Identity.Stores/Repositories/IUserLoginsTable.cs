using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IUserLoginsTable<TUser, TKey>
    where TUser : IdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<TUser?> GetAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default);
    Task DeleteUserLoginsAsync(TUser user, CancellationToken cancellationToken = default);
}
public interface IUserLoginsTable<TUser, TUserLogin, TKey> : IUserLoginsTable<TUser, TKey>
    where TUser : IdentityUser<TKey>, new()
    where TUserLogin : IdentityUserLogin<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TUserLogin userLogin, CancellationToken cancellationToken = default);
    Task<IList<TUserLogin>> GetAsync(TUser user, CancellationToken cancellationToken = default);
}
