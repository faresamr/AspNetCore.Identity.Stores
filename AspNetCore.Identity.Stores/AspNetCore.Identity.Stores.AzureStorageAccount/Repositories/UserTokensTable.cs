using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class UserTokensTable<TUserToken, TKey> : TableStorage, IUserTokensTable<TUserToken, TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserToken";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public UserTokensTable(IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
        }

        public async Task<IdentityResult> AddAsync(TUserToken userToken, CancellationToken cancellationToken)
        {
            return (await TableClient.UpsertEntityAsync(userToken.ToTableEntity(PartitionKey, GetHashKey(userToken)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, GetHashKey(userId, loginProvider, name), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<TUserToken> GetAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var response = await TableClient.GetEntityAsync<TableEntity>(PartitionKey, GetHashKey(userId, loginProvider, name), cancellationToken: cancellationToken);
            if (response.GetRawResponse().IsSuccess())
                return response.Value.ConvertTo<TUserToken>();
            else
                return null;
        }

        public async Task<IList<TUserToken>> GetAsync(TKey userId, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserToken<TKey>.UserId)} eq '{userId}'", cancellationToken: cancellationToken);
            List<TUserToken> userTokens = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                userTokens.Add(tableEntity.ConvertTo<TUserToken>());
            }
            return userTokens;
        }

        private static string GetHashKey(TUserToken userToken) => GetHashKey(userToken.UserId, userToken.LoginProvider, userToken.Name);
        private static string GetHashKey(TKey userId, string loginProvider, string name) => $"{userId}-{loginProvider}-{name}".GetHashString();
    }
}
