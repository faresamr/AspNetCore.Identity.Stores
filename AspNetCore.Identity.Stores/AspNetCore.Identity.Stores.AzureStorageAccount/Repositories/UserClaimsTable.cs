using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class UserClaimsTable<TUser, TUserClaim, TKey> : TableStorage, IUserClaimsTable<TUser, TUserClaim, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserClaim";

        public UserClaimsTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
        {
        }

        public Task<IdentityResult> AddAsync(TUserClaim userClaim, CancellationToken cancellationToken = default)
        {
            return AddAsync(PartitionKey, GetHashKey(userClaim), userClaim, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUserClaim userClaim, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(PartitionKey, GetHashKey(userClaim), cancellationToken: cancellationToken);
        }

        public async Task<IList<Claim>> GetAsync(TUser user, CancellationToken cancellationToken = default)
        {
            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and UserId eq {user.Id}");
            return (await QueryAsync<TUserClaim>(filter: filter, cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
        }

        public async Task<IList<TUser>> GetAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and ClaimType eq {claim.Type} and ClaimValue eq {claim.Value}");
            var queryResultsFilter = await QueryAsync<TUserClaim>(filter: filter, cancellationToken: cancellationToken);

            List<TUser> users = new();
            foreach (TUserClaim userClaim in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                users.Add(await QueryAsync<TUser>(UsersTable<TUser, TKey>.PartitionKey, ConvertToString(userClaim.UserId), cancellationToken: cancellationToken));
            }
            return users;
        }

        public Task DeleteUserClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and UserId eq {user.Id}");
            return DeleteBulkAsync(filter, cancellationToken);
        }

        private static string GetHashKey(TUserClaim userClaim) => $"{userClaim.UserId}-{userClaim.ClaimType}-{userClaim.ClaimValue}".GetHashString();
    }
}
