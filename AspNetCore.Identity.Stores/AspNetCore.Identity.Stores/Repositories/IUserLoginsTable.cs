using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserLoginsTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<TUser> GetAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken);
        Task DeleteUserLoginsAsync(TUser user, CancellationToken cancellationToken);
    }
    public interface IUserLoginsTable<TUser, TUserLogin, TKey> : IUserLoginsTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TUserLogin userLogin, CancellationToken cancellationToken);
        Task<IList<TUserLogin>> GetAsync(TUser user, CancellationToken cancellationToken);
    }
}
