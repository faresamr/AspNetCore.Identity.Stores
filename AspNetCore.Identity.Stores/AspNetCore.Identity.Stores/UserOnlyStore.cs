using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores
{
    public sealed class UserOnlyStore<TUser, TUserClaim, TUserLogin, TUserToken> : UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : IdentityUser<string>, new()
        where TUserClaim : IdentityUserClaim<string>, new()
        where TUserLogin : IdentityUserLogin<string>, new()
        where TUserToken : IdentityUserToken<string>, new()
    {
        public UserOnlyStore(IUsersTable<TUser, string> usersTable,
            IUserLoginsTable<TUser, TUserLogin, string> userLoginsTable,
            IUserClaimsTable<TUser, TUserClaim, string> userClaimsTable,
            IUserTokensTable<TUserToken, string> userTokensTable)
            : base(usersTable, userLoginsTable, userClaimsTable, userTokensTable)
        {
        }

    }
}
