using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserTokensTable<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> DeleteAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken);
        Task DeleteUserTokensAsync(TKey userId, CancellationToken cancellationToken);
    }
    public interface IUserTokensTable<TUserToken, TKey> : IUserTokensTable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TUserToken userToken, CancellationToken cancellationToken);
        Task<TUserToken> GetAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken);
        Task<IList<TUserToken>> GetAsync(TKey userId, CancellationToken cancellationToken);
    }
}
