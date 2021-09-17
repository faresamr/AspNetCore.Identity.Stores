using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories
{
    internal class UsersTable<TUser, TKey> : TableStorage, IUsersTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        internal const string PartitionKey = "User";
        private readonly IUserClaimsTable<TUser, TKey> userClaimsTable;
        private readonly IUserLoginsTable<TUser, TKey> userLoginsTable;
        private readonly IUserRolesTable<TKey> userRolesTable;
        private readonly IUserTokensTable<TKey> userTokensTable;

        public UsersTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options,
                          IUserClaimsTable<TUser, TKey> userClaimsTable,
                          IUserLoginsTable<TUser, TKey> userLoginsTable,
                          IUserRolesTable<TKey> userRolesTable,
                          IUserTokensTable<TKey> userTokensTable) : base(dataProtectionProvider, options)
        {
            this.userClaimsTable = userClaimsTable;
            this.userLoginsTable = userLoginsTable;
            this.userRolesTable = userRolesTable;
            this.userTokensTable = userTokensTable;
        }

        public Task<IdentityResult> AddAsync(TUser user, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, ConvertToString(user.Id), user, cancellationToken: cancellationToken);
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            await Task.WhenAll(new[]
            {
                userClaimsTable.DeleteUserClaimsAsync(user, cancellationToken),
                userLoginsTable.DeleteUserLoginsAsync(user, cancellationToken),
                userRolesTable.DeleteUserRolesAsync(user.Id, cancellationToken),
                userTokensTable.DeleteUserTokensAsync(user.Id, cancellationToken)
            });
            return await DeleteAsync(PartitionKey, ConvertToString(user.Id), cancellationToken: cancellationToken);
        }

        public IQueryable<TUser> Get() => Query<TUser>(BuildQuery(PartitionKey)).AsQueryable();

        public Task<TUser> GetAsync(TKey userId, CancellationToken cancellationToken)
        {
            return QueryAsync<TUser>(PartitionKey, ConvertToString(userId), cancellationToken: cancellationToken);
        }

        public async Task<TUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var qry = BuildQuery(PartitionKey, (nameof(IdentityUser<TKey>.NormalizedEmail), normalizedEmail));
            return (await QueryAsync<TUser>(qry, cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public async Task<TUser> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var qry = BuildQuery(PartitionKey, (nameof(IdentityUser<TKey>.NormalizedUserName), normalizedUserName));
            return (await QueryAsync<TUser>(qry, cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            return UpdateAsync(PartitionKey, ConvertToString(user.Id), user, cancellationToken);
        }
    }
}
