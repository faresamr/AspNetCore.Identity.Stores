using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores
{
    internal abstract class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> : IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IProtectedUserStore<TUser>
        where TUser : IdentityUser<string>, new()
        where TUserClaim : IdentityUserClaim<string>, new()
        where TUserLogin : IdentityUserLogin<string>, new()
        where TUserToken : IdentityUserToken<string>, new()
    {
        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";
        
        private readonly IUsersTable<TUser, string> usersTable;
        private readonly IUserLoginsTable<TUser, TUserLogin, string> userLoginsTable;
        private readonly IUserClaimsTable<TUser, TUserClaim, string> userClaimsTable;
        private readonly IUserTokensTable<TUserToken, string> userTokensTable;

        public UserStoreBase(IUsersTable<TUser, string> usersTable,
            IUserLoginsTable<TUser,TUserLogin,string> userLoginsTable,
            IUserClaimsTable<TUser,TUserClaim,string> userClaimsTable,
            IUserTokensTable<TUserToken, string> userTokensTable)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
            this.userLoginsTable = userLoginsTable ?? throw new ArgumentNullException(nameof(userLoginsTable));
            this.userClaimsTable = userClaimsTable ?? throw new ArgumentNullException(nameof(userClaimsTable));
            this.userTokensTable = userTokensTable ?? throw new ArgumentNullException(nameof(userTokensTable));
        }

        #region IQueryableUserStore
        public IQueryable<TUser> Users => usersTable.Get();
        #endregion

        void IDisposable.Dispose() { }

        #region IUserStore
        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) => usersTable.AddAsync(user, cancellationToken);

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) => usersTable.DeleteAsync(user, cancellationToken);

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => usersTable.GetAsync(userId, cancellationToken);

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => usersTable.GetByNormalizedUserNameAsync(normalizedUserName, cancellationToken);

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(Convert.ToString(user.Id));

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) => usersTable.UpdateAsync(user, cancellationToken);
        #endregion

        #region IUserPasswordStore
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        #endregion

        #region IUserEmailStore
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) => usersTable.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserPhoneNumberStore
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumberConfirmed);

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserSecurityStampStore
        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.SecurityStamp);
        #endregion

        #region IUserTwoFactorStore
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.TwoFactorEnabled);
        #endregion

        #region IUserLockoutStore
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnd);

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(++(user ?? throw new ArgumentNullException(nameof(user))).AccessFailedCount);

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.AccessFailedCount);

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnabled);

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            (user ?? throw new ArgumentNullException(nameof(user))).LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserLoginStore
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login is null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            TUserLogin userLogin = new();
            userLogin.UserId = user.Id;
            userLogin.LoginProvider = login.LoginProvider;
            userLogin.ProviderKey = login.ProviderKey;
            userLogin.ProviderDisplayName = login.ProviderDisplayName;
            return userLoginsTable.AddAsync(userLogin, cancellationToken);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) => userLoginsTable.DeleteAsync(user ?? throw new ArgumentNullException(nameof(user)), loginProvider, providerKey, cancellationToken);

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken) => (await userLoginsTable.GetAsync(user, cancellationToken)).Select(i => new UserLoginInfo(i.LoginProvider, i.ProviderKey, i.ProviderDisplayName)).ToList();

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) => userLoginsTable.GetAsync(loginProvider, providerKey, cancellationToken);
        #endregion

        #region IUserClaimStore
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken) => userClaimsTable.GetAsync(user, cancellationToken);

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.WhenAll(claims.Select(i =>
            {
                TUserClaim userClaim = new();
                userClaim.UserId = user.Id;
                userClaim.InitializeFromClaim(i);
                return userClaimsTable.AddAsync(userClaim,cancellationToken);
            }));
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            TUserClaim userClaim = new();
            userClaim.UserId = user.Id;
            userClaim.InitializeFromClaim(claim);
            await userClaimsTable.DeleteAsync(userClaim, cancellationToken);

            userClaim.InitializeFromClaim(newClaim);
            await userClaimsTable.AddAsync(userClaim, cancellationToken);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.WhenAll(claims.Select(i =>
            {
                TUserClaim userClaim = new();
                userClaim.UserId = user.Id;
                userClaim.InitializeFromClaim(i);
                return userClaimsTable.DeleteAsync(userClaim, cancellationToken);
            }));
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) => userClaimsTable.GetAsync(claim, cancellationToken);
        #endregion

        #region IUserAuthenticationTokenStore
        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            TUserToken userToken = new();
            userToken.UserId = user.Id;
            userToken.LoginProvider = loginProvider;
            userToken.Name = name;
            userToken.Value = value;
            return userTokensTable.AddAsync(userToken, cancellationToken);
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) => userTokensTable.DeleteAsync(user.Id, loginProvider, name, cancellationToken);

        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) => (await userTokensTable.GetAsync(user.Id, loginProvider, name, cancellationToken))?.Value;
        #endregion

        #region IUserAuthenticatorKeyStore
        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken) => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken) => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        #endregion

        #region IUserTwoFactorRecoveryCodeStore
        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }
        #endregion
    }
}
