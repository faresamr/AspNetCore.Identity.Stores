using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IUserClaimsTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IList<Claim>> GetAsync(TUser user, CancellationToken cancellationToken = default);
        Task<IList<TUser>> GetAsync(Claim claim, CancellationToken cancellationToken = default);
        Task DeleteUserClaimsAsync(TUser user, CancellationToken cancellationToken);
    }
    public interface IUserClaimsTable<TUser, TUserClaim, TKey> : IUserClaimsTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TUserClaim userClaim, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteAsync(TUserClaim userClaim, CancellationToken cancellationToken = default);
    }
}
