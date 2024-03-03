using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores;

internal sealed class UserStore<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken> : UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>,
    IUserRoleStore<TUser>
    where TUser : IdentityUser<string>, new()
    where TRole : IdentityRole<string>, new()
    where TUserClaim : IdentityUserClaim<string>, new()
    where TUserRole : IdentityUserRole<string>, new()
    where TUserLogin : IdentityUserLogin<string>, new()
    where TUserToken : IdentityUserToken<string>, new()
{
    private readonly IUsersTable<TUser, string> usersTable;
    private readonly IRolesTable<TRole, string> rolesTable;
    private readonly IUserRolesTable<TUserRole, string> userRolesTable;

    public UserStore(IUsersTable<TUser, string> usersTable,
        IRolesTable<TRole, string> rolesTable,
        IUserLoginsTable<TUser, TUserLogin, string> userLoginsTable,
        IUserClaimsTable<TUser, TUserClaim, string> userClaimsTable,
        IUserRolesTable<TUserRole, string> userRolesTable,
        IUserTokensTable<TUserToken, string> userTokensTable)
        : base(usersTable, userLoginsTable, userClaimsTable, userTokensTable)
    {
        this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
        this.rolesTable = rolesTable ?? throw new ArgumentNullException(nameof(rolesTable));
        this.userRolesTable = userRolesTable ?? throw new ArgumentNullException(nameof(userRolesTable));
    }

    #region IUserRoleStore
    public async  Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        var roleEntity = await rolesTable.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken) ?? throw new InvalidOperationException($"Role Not Found");
        TUserRole userRole = new()
        {
            UserId = user.Id,
            RoleId = roleEntity.Id
        };
        await userRolesTable.AddAsync(userRole, cancellationToken);
    }

    public async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        var roleEntity = await rolesTable.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken) ?? throw new InvalidOperationException($"Role Not Found");
        await userRolesTable.DeleteAsync(user.Id, roleEntity.Id, cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        var roleIds = (await userRolesTable.GetRolesAsync(user.Id, cancellationToken)).Select(i => i.RoleId);
        if (roleIds.Any())
        {
            IEnumerable<TRole> roles = await rolesTable.GetAsync(cancellationToken);
            return roles
                .Join(roleIds, role => role.Id, id => id, (role, id) => role.Name)
                .OfType<string>()
                .ToList();
        }
        else
            return Enumerable.Empty<string>().ToList();
    }

    public async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        var roleEntity = await rolesTable.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken) ?? throw new InvalidOperationException($"Role Not Found");
        return (await userRolesTable.GetRolesAsync(user.Id, cancellationToken)).Any(i => i.RoleId == roleEntity.Id);
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        var roleEntity = await rolesTable.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken) ?? throw new InvalidOperationException($"Role Not Found");
        return (await userRolesTable.GetUsersAsync(roleEntity.Id, cancellationToken)).Select(async i => await usersTable.GetAsync(i.UserId, cancellationToken)).Select(i => i.Result).OfType<TUser>().ToList();
    }
    #endregion
}
