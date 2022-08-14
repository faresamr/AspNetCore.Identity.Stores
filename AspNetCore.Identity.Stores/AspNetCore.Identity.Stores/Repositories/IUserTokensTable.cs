using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IUserTokensTable<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> DeleteAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken = default);
    Task DeleteUserTokensAsync(TKey userId, CancellationToken cancellationToken = default);
}
public interface IUserTokensTable<TUserToken, TKey> : IUserTokensTable<TKey>
    where TUserToken : IdentityUserToken<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TUserToken userToken, CancellationToken cancellationToken = default);
    Task<TUserToken?> GetAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken = default);
    Task<IList<TUserToken>> GetAsync(TKey userId, CancellationToken cancellationToken = default);
}
