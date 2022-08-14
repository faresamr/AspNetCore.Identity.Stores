using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;

internal class UserClaimsTable<TUser, TUserClaim, TKey> : CosmosContainer, IUserClaimsTable<TUser, TUserClaim, TKey>
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
        var qry = BuildQuery(PartitionKey, (nameof(IdentityUserClaim<TKey>.UserId), user.Id));
        return (await QueryAsync<TUserClaim>(qry, cancellationToken: cancellationToken)).Select(i => i.ToClaim()).ToList();
    }

    public async Task<IList<TUser>> GetAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        var qry = BuildQuery(PartitionKey,
            (nameof(IdentityUserClaim<TKey>.ClaimType), claim.Type),
            (nameof(IdentityUserClaim<TKey>.ClaimValue), claim.Value));
        var queryResultsFilter = await QueryAsync<TUserClaim>(qry, cancellationToken: cancellationToken);

        List<TUser> users = new();
        foreach (TUserClaim userClaim in queryResultsFilter)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TUser? user = await QueryAsync<TUser>(UsersTable<TUser, TKey>.PartitionKey, ConvertToString(userClaim.UserId), cancellationToken: cancellationToken);
            if (user is not null)
                users.Add(user);
        }
        return users;
    }

    private static string GetHashKey(TUserClaim userClaim) => $"{userClaim.UserId}-{userClaim.ClaimType}-{userClaim.ClaimValue}".GetHashString();

    public Task DeleteUserClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey, (nameof(IdentityUserClaim<TKey>.UserId), user.Id));
        return DeleteBulkAsync(qry, cancellationToken);
    }
}
