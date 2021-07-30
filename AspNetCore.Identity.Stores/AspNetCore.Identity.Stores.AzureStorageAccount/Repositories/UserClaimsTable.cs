using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
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
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";
        private readonly IUsersTable<TUser, TKey> usersTable;

        public UserClaimsTable(IUsersTable<TUser, TKey> usersTable, IDataProtectionProvider dataProtectionProvider, IOptions<StorageAccountOptions> options) : base(dataProtectionProvider, options, IdentityTable)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
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
            return (await QueryAsync<TUserClaim>(filter: $"{PartitionFilter} and {nameof(IdentityUserClaim<TKey>.UserId)} eq '{user.Id}'", cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
        }

        public async Task<IList<TUser>> GetAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            var queryResultsFilter= await QueryAsync<TUserClaim>(filter: $"{PartitionFilter} and {nameof(IdentityUserClaim<TKey>.ClaimType)} eq '{claim.Type}' and {nameof(IdentityUserClaim<TKey>.ClaimValue)} eq '{claim.Value}'", cancellationToken: cancellationToken);

            List<TUser> users = new();
            foreach (TUserClaim userClaim in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                users.Add(await usersTable.GetAsync(userClaim.UserId, cancellationToken));
            }
            return users;
        }

        private static string GetHashKey(TUserClaim userClaim) => $"{userClaim.UserId}-{userClaim.ClaimType}-{userClaim.ClaimValue}".GetHashString();
    }
}
