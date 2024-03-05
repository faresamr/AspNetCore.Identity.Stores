using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores
{
    internal class RoleStore<TRole, TRoleClaim> : IRoleStore<TRole>,
        IRoleClaimStore<TRole>,
        IQueryableRoleStore<TRole>
        where TRole : IdentityRole<string>, new()
        where TRoleClaim : IdentityRoleClaim<string>, new()
    {
        private readonly IRolesTable<TRole, string> rolesTable;
        private readonly IRoleClaimsTable<TRole, TRoleClaim, string> roleClaimsTable;

        public RoleStore(IRolesTable<TRole, string> rolesTable, IRoleClaimsTable<TRole, TRoleClaim, string> roleClaimsTable)
        {
            this.rolesTable = rolesTable ?? throw new ArgumentNullException(nameof(rolesTable));
            this.roleClaimsTable = roleClaimsTable ?? throw new ArgumentNullException(nameof(roleClaimsTable));
        }

        #region IQueryableRoleStore
        public IQueryable<TRole> Roles => rolesTable.Get();
        #endregion

        #region IRoleStore
        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken) => rolesTable.AddAsync(role, cancellationToken);

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken) => rolesTable.DeleteAsync(role, cancellationToken);

        public void Dispose() { }

        public Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken) => rolesTable.GetAsync(roleId, cancellationToken);

        public Task<TRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) => rolesTable.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken);

        public Task<string?> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(role.NormalizedName);

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(Convert.ToString(role.Id));

        public Task<string?> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name);

        public Task SetNormalizedRoleNameAsync(TRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken) => rolesTable.UpdateAsync(role, cancellationToken);
        #endregion

        #region IRoleClaimStore
        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            TRoleClaim roleClaim = new() { RoleId = role.Id };
            roleClaim.InitializeFromClaim(claim);
            return roleClaimsTable.AddAsync(roleClaim, cancellationToken);
        }

        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            return roleClaimsTable.GetAsync(role, cancellationToken);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            TRoleClaim roleClaim = new() { RoleId = role.Id };
            roleClaim.InitializeFromClaim(claim);
            return roleClaimsTable.DeleteAsync(roleClaim, cancellationToken);
        }
        #endregion
    }
}
