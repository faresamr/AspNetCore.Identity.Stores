using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

        public UserClaimsTable(IUsersTable<TUser, TKey> usersTable, IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
        }

        public async Task<IdentityResult> AddAsync(TUserClaim userClaim, CancellationToken cancellationToken = default)
        {
            return (await TableClient.UpsertEntityAsync(userClaim.ToTableEntity(PartitionKey, GetHashKey(userClaim)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TUserClaim userClaim, CancellationToken cancellationToken = default)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, GetHashKey(userClaim), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IList<Claim>> GetAsync(TUser user, CancellationToken cancellationToken = default)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserClaim<TKey>.UserId)} eq '{user.Id}'", cancellationToken: cancellationToken);
            List<Claim> claims = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                claims.Add(tableEntity.ConvertTo<TUserClaim>().ToClaim());
            }
            return claims;
        }

        public async Task<IList<TUser>> GetAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserClaim<TKey>.ClaimType)} eq '{claim.Type}' and {nameof(IdentityUserClaim<TKey>.ClaimValue)} eq '{claim.Value}'", cancellationToken: cancellationToken);
            List<TUser> users = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                TUserClaim userClaim = tableEntity.ConvertTo<TUserClaim>();

                users.Add(await usersTable.GetAsync(userClaim.UserId, cancellationToken));
            }
            return users;
        }

        private static string GetHashKey(TUserClaim userClaim) => $"{userClaim.UserId}-{userClaim.ClaimType}-{userClaim.ClaimValue}".GetHashString();
    }
}
