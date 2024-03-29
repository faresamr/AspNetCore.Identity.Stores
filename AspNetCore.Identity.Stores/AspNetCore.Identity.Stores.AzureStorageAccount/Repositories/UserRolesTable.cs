﻿using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Repositories;

internal class UserRolesTable<TUserRole, TKey> : TableStorage, IUserRolesTable<TUserRole, TKey>
    where TUserRole : IdentityUserRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string PartitionKey = "UserRole";

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
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and RoleId eq {roleId}");
        return (await QueryAsync<TUserRole>(filter: filter, cancellationToken: cancellationToken)).ToList();
    }

    public async Task<IList<TUserRole>> GetRolesAsync(TKey userId, CancellationToken cancellationToken)
    {
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and UserId eq {userId}");
        return (await QueryAsync<TUserRole>(filter: filter, cancellationToken: cancellationToken)).ToList();
    }

    public Task DeleteUserRolesAsync(TKey userId, CancellationToken cancellationToken)
    {
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and UserId eq {userId}");
        return DeleteBulkAsync(filter, cancellationToken);
    }
    public Task DeleteRoleUsersAsync(TKey roleId, CancellationToken cancellationToken)
    {
        string filter = TableClient.CreateQueryFilter($"PartitionKey eq {PartitionKey} and RoleId eq {roleId}");
        return DeleteBulkAsync(filter, cancellationToken);
    }

    private static string GetHashKey(TUserRole userRole) => GetHashKey(userRole.UserId, userRole.RoleId);
    private static string GetHashKey(TKey userId, TKey roleId) => $"{userId}-{roleId}".GetHashString();
}
