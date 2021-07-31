﻿using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
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

        public UserTokensTable(IDataProtectionProvider dataProtectionProvider, IOptions<StorageAccountOptions> options) : base(dataProtectionProvider, options, IdentityTable)
        {
        }

        public Task<IdentityResult> AddAsync(TUserToken userToken, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, GetHashKey(userToken), userToken, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return DeleteAsync(PartitionKey, GetHashKey(userId, loginProvider, name), cancellationToken: cancellationToken);
        }

        public Task<TUserToken> GetAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return QueryAsync<TUserToken>(PartitionKey, GetHashKey(userId, loginProvider, name), cancellationToken: cancellationToken);
        }

        public Task<IList<TUserToken>> GetAsync(TKey userId, CancellationToken cancellationToken)
        {
            return QueryAsync<TUserToken>(filter: $"{PartitionFilter} and {nameof(IdentityUserToken<TKey>.UserId)} eq '{userId}'", cancellationToken: cancellationToken);
        }

        private static string GetHashKey(TUserToken userToken) => GetHashKey(userToken.UserId, userToken.LoginProvider, userToken.Name);
        private static string GetHashKey(TKey userId, string loginProvider, string name) => $"{userId}-{loginProvider}-{name}".GetHashString();
    }
}