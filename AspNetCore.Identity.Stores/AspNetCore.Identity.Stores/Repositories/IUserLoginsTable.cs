using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserLoginsTable<TUser, TUserLogin, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> CreateAsync(TUserLogin userLogin, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken);
        Task<IList<TUserLogin>> GetAsync(TUser user, CancellationToken cancellationToken);
        Task<TUser> GetAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
    }
}
