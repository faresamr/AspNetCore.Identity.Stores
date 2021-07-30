using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores
{
    internal sealed class UserStore<TUser, TUserClaim, TUserRole, TUserLogin, TUserToken> : UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>,
        IUserRoleStore<TUser>
        where TUser : IdentityUser<string>, new()
        where TUserClaim : IdentityUserClaim<string>, new()
        where TUserRole : IdentityUserRole<string>, new()
        where TUserLogin : IdentityUserLogin<string>, new()
        where TUserToken : IdentityUserToken<string>, new()
    {
        private readonly IUsersTable<TUser, string> usersTable;
        private readonly IUserRolesTable<TUserRole, string> userRolesTable;

        public UserStore(IUsersTable<TUser, string> usersTable,
            IUserLoginsTable<TUser, TUserLogin, string> userLoginsTable,
            IUserClaimsTable<TUser, TUserClaim, string> userClaimsTable,
            IUserRolesTable<TUserRole, string> userRolesTable,
            IUserTokensTable<TUserToken, string> userTokensTable)
            : base(usersTable, userLoginsTable, userClaimsTable, userTokensTable)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
            this.userRolesTable = userRolesTable ?? throw new ArgumentNullException(nameof(userRolesTable));
        }

        #region IUserRoleStore
        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            TUserRole userRole = new();
            userRole.UserId = user.Id;
            userRole.RoleId = roleName;
            return userRolesTable.AddAsync(userRole, cancellationToken);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) => userRolesTable.DeleteAsync(user.Id, roleName, cancellationToken);

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            return (await userRolesTable.GetRolesAsync(user.Id, cancellationToken)).Select(i => i.RoleId).ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            return (await userRolesTable.GetRolesAsync(user.Id, cancellationToken)).Any(i => i.RoleId == roleName);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) => (await userRolesTable.GetUsersAsync(roleName, cancellationToken)).Select(async i => await usersTable.GetAsync(i.UserId, cancellationToken)).Select(i => i.Result).ToList();
        #endregion
    }
}
