using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores
{
    //public class UserStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> : IUserStore<TUser>,
    //    IUserLoginStore<TUser>,
    //    IUserClaimStore<TUser>,
    //    IUserRoleStore<TUser>,
    //    IUserPasswordStore<TUser>,
    //    IUserSecurityStampStore<TUser>,
    //    IUserEmailStore<TUser>,
    //    IUserLockoutStore<TUser>,
    //    IUserPhoneNumberStore<TUser>,
    //    IQueryableUserStore<TUser>,
    //    IUserTwoFactorStore<TUser>,
    //    IUserAuthenticationTokenStore<TUser>,
    //    IUserAuthenticatorKeyStore<TUser>,
    //    IUserTwoFactorRecoveryCodeStore<TUser>
    //    where TUser : IdentityUser<TKey>, new()
    //    where TKey : IEquatable<TKey>
    //    where TUserClaim : IdentityUserClaim<TKey>, new()
    //    where TUserLogin : IdentityUserLogin<TKey>, new()
    //    where TUserToken : IdentityUserToken<TKey>, new()
    //{
    //    public UserStore(IdentityStorageTable identityStorageTable)
    //    {
    //        this.identityStorageTable = identityStorageTable ?? throw new ArgumentNullException(nameof(identityStorageTable));
    //    }

    //    #region IQueryableUserStore
    //    public IQueryable<TUser> Users
    //    {
    //        get
    //        {
    //            Pageable<TableEntity> queryResultsFilter = identityStorageTable.Querey<TableEntity>(filter: $"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.User}'");
    //            return queryResultsFilter.Select(i => i.ConvertTo<TUser>()).AsQueryable();
    //        }
    //    }
    //    #endregion

    //    public void Dispose() { }

    //    #region IUserStore
    //    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) => identityStorageTable.InsertAsync(user ?? throw new ArgumentNullException(nameof(user)), IdentityStorageTable.PartitionKey.Role, user.Id, cancellationToken);

    //    public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) => identityStorageTable.DeleteAsync(IdentityStorageTable.PartitionKey.User, (user ?? throw new ArgumentNullException(nameof(user))).Id, cancellationToken);

    //    public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => identityStorageTable.GetAsync<TUser>(IdentityStorageTable.PartitionKey.User, userId, cancellationToken);

    //    public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => identityStorageTable.GetAsync<TUser>($"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.User}' and {nameof(IdentityUser.NormalizedUserName)} eq '{normalizedUserName}'", cancellationToken);

    //    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

    //    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(Convert.ToString(user.Id));

    //    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

    //    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).NormalizedUserName = normalizedName;
    //        return Task.CompletedTask;
    //    }

    //    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).UserName = userName;
    //        return Task.CompletedTask;
    //    }

    //    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    //    {
    //        return identityStorageTable.UpdateAsync(user ?? throw new ArgumentNullException(nameof(user)), IdentityStorageTable.PartitionKey.User, user.Id, cancellationToken);
    //    }
    //    #endregion

    //    #region IUserPasswordStore
    //    public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).PasswordHash = passwordHash;
    //        return Task.CompletedTask;
    //    }

    //    public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

    //    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    //    #endregion

    //    #region IUserEmailStore
    //    public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).Email = email;
    //        return Task.CompletedTask;
    //    }

    //    public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

    //    public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);

    //    public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).EmailConfirmed = confirmed;
    //        return Task.CompletedTask;
    //    }

    //    public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) => identityStorageTable.GetAsync<TUser>($"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.User}' and {nameof(IdentityUser.NormalizedEmail)} eq '{normalizedEmail}'", cancellationToken);

    //    public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);

    //    public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).NormalizedEmail = normalizedEmail;
    //        return Task.CompletedTask;
    //    }
    //    #endregion

    //    #region IUserPhoneNumberStore
    //    public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).PhoneNumber = phoneNumber;
    //        return Task.CompletedTask;
    //    }

    //    public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumber);

    //    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumberConfirmed);

    //    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).PhoneNumberConfirmed = confirmed;
    //        return Task.CompletedTask;
    //    }
    //    #endregion

    //    #region IUserSecurityStampStore
    //    public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).SecurityStamp = stamp;
    //        return Task.CompletedTask;
    //    }

    //    public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.SecurityStamp);
    //    #endregion

    //    #region IUserTwoFactorStore
    //    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).TwoFactorEnabled = enabled;
    //        return Task.CompletedTask;
    //    }

    //    public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.TwoFactorEnabled);
    //    #endregion

    //    #region IUserLockoutStore
    //    public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnd);

    //    public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).LockoutEnd = lockoutEnd;
    //        return Task.CompletedTask;
    //    }

    //    public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(++(user ?? throw new ArgumentNullException(nameof(user))).AccessFailedCount);

    //    public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).AccessFailedCount = 0;
    //        return Task.CompletedTask;
    //    }

    //    public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.AccessFailedCount);

    //    public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnabled);

    //    public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    //    {
    //        (user ?? throw new ArgumentNullException(nameof(user))).LockoutEnabled = enabled;
    //        return Task.CompletedTask;
    //    }
    //    #endregion

    //    #region IUserLoginStore
    //    public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
    //    {
    //        if (user is null)
    //        {
    //            throw new ArgumentNullException(nameof(user));
    //        }

    //        if (login is null)
    //        {
    //            throw new ArgumentNullException(nameof(login));
    //        }

    //        IdentityUserLoginInfo<TKey, TUser> identityUserLogin = new(user, login);
    //        var entity = identityUserLogin.ToTableEntity(IdentityStorageTable.PartitionKey.UserLogIn.ToString(), Guid.NewGuid().ToString());
    //        return identityStorageTable.InsertAsync(entity, cancellationToken: cancellationToken);
    //    }

    //    public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    //    {
    //        if (user is null)
    //        {
    //            throw new ArgumentNullException(nameof(user));
    //        }

    //        return identityStorageTable.DeleteAsync($"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.UserLogIn}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.LoginProvider)} eq '{loginProvider}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.ProviderKey)} eq '{providerKey}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.UserId)} eq '{user.Id}'", cancellationToken);
    //    }

    //    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
    //    {
    //        var queryResultsFilter = identityStorageTable.QuereyAsync<TableEntity>($"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.UserLogIn}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.UserId)} eq '{user.Id}'", cancellationToken);
    //        List<UserLoginInfo> userLoginInfos = new();
    //        await foreach (TableEntity tableEntity in queryResultsFilter)
    //        {
    //            cancellationToken.ThrowIfCancellationRequested();

    //            userLoginInfos.Add(new(
    //                tableEntity[nameof(IdentityUserLoginInfo<TKey, TUser>.LoginProvider)] as string,
    //                tableEntity[nameof(IdentityUserLoginInfo<TKey, TUser>.ProviderKey)] as string,
    //                tableEntity[nameof(IdentityUserLoginInfo<TKey, TUser>.ProviderDisplayName)] as string));
    //        }
    //        return userLoginInfos;
    //    }

    //    public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    //    {
    //        var userLogin = await identityStorageTable.GetAsync<IdentityUserLoginInfo<TKey, TUser>>($"{nameof(TableEntity.PartitionKey)} eq '{IdentityStorageTable.PartitionKey.UserLogIn}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.LoginProvider)} eq '{loginProvider}' and {nameof(IdentityUserLoginInfo<TKey, TUser>.ProviderKey)} eq '{providerKey}'", cancellationToken);
    //        if (userLogin is not null)
    //            return await identityStorageTable.GetAsync<TUser>(IdentityStorageTable.PartitionKey.User, Convert.ToString(userLogin.UserId), cancellationToken);
    //        else
    //            return null;

    //    }
    //    #endregion
    //}
}
