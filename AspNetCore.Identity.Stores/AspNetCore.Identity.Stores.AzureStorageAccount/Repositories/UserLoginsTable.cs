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
    internal class UserLoginsTable<TUser, TUserLogin, TKey> : TableStorage, IUserLoginsTable<TUser, TUserLogin, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserLogin";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";
        private readonly IUsersTable<TUser, TKey> usersTable;

        public UserLoginsTable(IUsersTable<TUser, TKey> usersTable, IOptions<StorageAccountOptions> options) : base(options, IdentityTable)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
        }

        public async Task<IdentityResult> CreateAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            return (await TableClient.UpsertEntityAsync(userLogin.ToTableEntity(PartitionKey, GetHashKey(userLogin)), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return (await TableClient.DeleteEntityAsync(PartitionKey, GetHashKey(loginProvider, providerKey), cancellationToken: cancellationToken)).ToIdentityResult();
        }

        public async Task<IList<TUserLogin>> GetAsync(TUser user, CancellationToken cancellationToken)
        {
            AsyncPageable<TableEntity> queryResultsFilter = TableClient.QueryAsync<TableEntity>(filter: $"{PartitionFilter} and {nameof(IdentityUserLogin<TKey>.UserId)} eq '{user.Id}'", cancellationToken: cancellationToken);
            List<TUserLogin> userLogins = new();
            await foreach (TableEntity tableEntity in queryResultsFilter)
            {
                cancellationToken.ThrowIfCancellationRequested();

                userLogins.Add(tableEntity.ConvertTo<TUserLogin>());
            }
            return userLogins;
        }

        public async Task<TUser> GetAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var response = await TableClient.GetEntityAsync<TableEntity>(PartitionKey, GetHashKey(loginProvider, providerKey), cancellationToken: cancellationToken);
            if (response.GetRawResponse().IsSuccess())
            {
                TUserLogin userLogin = response.Value.ConvertTo<TUserLogin>();
                if (userLogin is not null)
                    return await usersTable.GetAsync(userLogin.UserId, cancellationToken);
            }
            return null;
        }

        private static string GetHashKey(TUserLogin userLogin) => GetHashKey(userLogin.LoginProvider, userLogin.ProviderKey);
        private static string GetHashKey(string loginProvider, string providerKey) => $"{loginProvider}-{providerKey}".GetHashString();
    }
}
