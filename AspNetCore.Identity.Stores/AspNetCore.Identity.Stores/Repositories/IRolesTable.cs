using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.Repositories
{
    public interface IRolesTable<TRole, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> AddAsync(TRole role, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken);
        Task<TRole> GetAsync(TKey roleId, CancellationToken cancellationToken);
        Task<IEnumerable<TRole>> GetAsync(CancellationToken cancellationToken);
        Task<TRole> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken);
        IQueryable<TRole> Get();
    }
}
