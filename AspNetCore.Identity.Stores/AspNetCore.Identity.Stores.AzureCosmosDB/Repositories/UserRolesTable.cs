using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
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
    internal class UserRolesTable<TUserRole, TKey> : TableStorage, IUserRolesTable<TUserRole, TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserRole";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public UserRolesTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
        {
        }

        public Task<IdentityResult> AddAsync(TUserRole userToken, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, GetHashKey(userToken), userToken, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
        {
            return DeleteAsync(PartitionKey, GetHashKey(userId, roleId), cancellationToken: cancellationToken);
        }

        public async Task<IList<TUserRole>> GetUsersAsync(TKey roleId, CancellationToken cancellationToken)
        {
            string qry = BuildQuery(PartitionKey, new KeyValuePair<string, object>(nameof(IdentityUserRole<TKey>.RoleId), roleId));
            return (await QueryAsync<TUserRole>(filter: qry, cancellationToken: cancellationToken)).ToList();
        }

        public async Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken)
        {
            string qry = BuildQuery(PartitionKey, new KeyValuePair<string, object>(nameof(IdentityUserRole<TKey>.UserId), userId));
            return (await QueryAsync<TUserRole>(filter: qry, cancellationToken: cancellationToken)).ToList();
        }

        private static string GetHashKey(TUserRole userRole) => GetHashKey(userRole.UserId, userRole.RoleId);
        private static string GetHashKey(TKey userId, TKey roleId) => $"{userId}-{roleId}".GetHashString();
    }
}
