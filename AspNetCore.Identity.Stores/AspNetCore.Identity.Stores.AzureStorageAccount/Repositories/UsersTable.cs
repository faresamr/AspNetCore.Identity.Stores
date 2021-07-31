﻿using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using AspNetCore.Identity.Stores.Repositories;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories
{
    internal class UsersTable<TUser, TKey> : TableStorage, IUsersTable<TUser, TKey>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "User";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";
        public UsersTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options, IdentityTable)
        {
        }

        public Task<IdentityResult> AddAsync(TUser user, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, ConvertToString(user.Id), user, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            return DeleteAsync(PartitionKey, ConvertToString(user.Id), cancellationToken: cancellationToken);
        }

        public IQueryable<TUser> Get() => Query<TUser>().AsQueryable();

        public Task<TUser> GetAsync(TKey userId, CancellationToken cancellationToken)
        {
            return QueryAsync<TUser>(PartitionKey, ConvertToString(userId), cancellationToken: cancellationToken); ;
        }

        public async Task<TUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return (await QueryAsync<TUser>(filter: $"{PartitionFilter} and {nameof(IdentityUser<TKey>.NormalizedEmail)} eq '{normalizedEmail}'", cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public async Task<TUser> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return (await QueryAsync<TUser>(filter: $"{PartitionFilter} and {nameof(IdentityUser<TKey>.NormalizedUserName)} eq '{normalizedUserName}'", cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            return UpdateAsync(PartitionKey, ConvertToString(user.Id), user, cancellationToken);
        }
    }
}
