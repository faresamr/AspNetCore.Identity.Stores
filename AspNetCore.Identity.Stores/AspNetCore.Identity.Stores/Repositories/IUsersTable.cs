using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUsersTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken);
        Task<TUser> GetAsync(TKey userId, CancellationToken cancellationToken);
        Task<TUser> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken);
        Task<TUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
        IQueryable<TUser> Get();
    }
}
