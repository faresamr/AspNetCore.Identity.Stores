using AspNetCore.Identity.Stores.Repositories;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;

internal class UserTokensTable<TUserToken, TKey> : CosmosContainer, IUserTokensTable<TUserToken, TKey>
    where TUserToken : IdentityUserToken<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string PartitionKey = "UserToken";

    public UserTokensTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
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

    public Task<TUserToken?> GetAsync(TKey userId, string loginProvider, string name, CancellationToken cancellationToken)
    {
        return QueryAsync<TUserToken>(PartitionKey, GetHashKey(userId, loginProvider, name), cancellationToken: cancellationToken);
    }

    public Task<IList<TUserToken>> GetAsync(TKey userId, CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey, (nameof(IdentityUserToken<TKey>.UserId), userId));
        return QueryAsync<TUserToken>(qry, cancellationToken: cancellationToken);
    }

    private static string GetHashKey(TUserToken userToken) => GetHashKey(userToken.UserId, userToken.LoginProvider, userToken.Name);
    private static string GetHashKey(TKey userId, string loginProvider, string name) => $"{userId}-{loginProvider}-{name}".GetHashString();

    public Task DeleteUserTokensAsync(TKey userId, CancellationToken cancellationToken)
    {
        var qry = BuildQuery(PartitionKey, (nameof(IdentityUserToken<TKey>.UserId), userId));
        return DeleteBulkAsync(qry, cancellationToken);
    }
}
