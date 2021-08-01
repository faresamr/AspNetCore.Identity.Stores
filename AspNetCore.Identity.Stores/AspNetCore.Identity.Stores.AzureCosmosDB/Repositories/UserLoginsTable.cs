﻿using AspNetCore.Identity.Stores.Repositories;
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
    internal class UserLoginsTable<TUser, TUserLogin, TKey> : TableStorage, IUserLoginsTable<TUser, TUserLogin, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "UserLogin";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";
        private readonly IUsersTable<TUser, TKey> usersTable;

        public UserLoginsTable(IUsersTable<TUser, TKey> usersTable, IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
        {
            this.usersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
        }

        public Task<IdentityResult> AddAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, GetHashKey(userLogin), userLogin, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return DeleteAsync(PartitionKey, GetHashKey(loginProvider, providerKey), cancellationToken: cancellationToken);
        }

        public async Task<IList<TUserLogin>> GetAsync(TUser user, CancellationToken cancellationToken)
        {
            string qry = BuildQuery(PartitionKey, new KeyValuePair<string, object>(nameof(IdentityUserLogin<TKey>.UserId), user.Id));
            return (await QueryAsync<TUserLogin>(filter: qry, cancellationToken: cancellationToken)).ToList();
        }

        public async Task<TUser> GetAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userLogin = await QueryAsync<TUserLogin>(PartitionKey, GetHashKey(loginProvider, providerKey), cancellationToken: cancellationToken);
            if (userLogin is not null)
                return await usersTable.GetAsync(userLogin.UserId, cancellationToken);
            else
                return null;
        }

        private static string GetHashKey(TUserLogin userLogin) => GetHashKey(userLogin.LoginProvider, userLogin.ProviderKey);
        private static string GetHashKey(string loginProvider, string providerKey) => $"{loginProvider}-{providerKey}".GetHashString();
    }
}