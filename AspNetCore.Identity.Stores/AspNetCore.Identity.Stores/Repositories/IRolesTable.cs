using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Repositories;

public interface IRolesTable<TRole, TKey>
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    Task<IdentityResult> AddAsync(TRole role, CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default);
    Task<TRole?> GetAsync(TKey roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TRole>> GetAsync(CancellationToken cancellationToken = default);
    Task<TRole?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    IQueryable<TRole> Get();
}
