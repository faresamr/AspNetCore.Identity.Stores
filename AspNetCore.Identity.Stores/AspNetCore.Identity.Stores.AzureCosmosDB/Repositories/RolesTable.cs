using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories
{
    internal class RolesTable<TRole, TKey> : TableStorage, IRolesTable<TRole, TKey>
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private const string PartitionKey = "Role";
        private readonly string PartitionFilter = $"{nameof(TableEntity.PartitionKey)} eq '{PartitionKey}'";

        public RolesTable(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options) : base(dataProtectionProvider, options)
        {
        }

        public Task<IdentityResult> AddAsync(TRole role, CancellationToken cancellationToken)
        {
            return AddAsync(PartitionKey, ConvertToString(role.Id), role, cancellationToken: cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            return DeleteAsync(PartitionKey, ConvertToString(role.Id), cancellationToken: cancellationToken);
        }

        public IQueryable<TRole> Get() => Query<TRole>().AsQueryable();

        public Task<TRole> GetAsync(TKey roleId, CancellationToken cancellationToken)
        {
            return QueryAsync<TRole>(PartitionKey, ConvertToString(roleId), cancellationToken: cancellationToken);
        }
        public async Task<IEnumerable<TRole>> GetAsync(CancellationToken cancellationToken)
        {
            return await QueryAsync<TRole>(filter: PartitionFilter, cancellationToken: cancellationToken);
        }

        public async Task<TRole> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken)
        {
            string qry = BuildQuery(PartitionKey, new KeyValuePair<string, object>(nameof(IdentityRole<TKey>.NormalizedName), normalizedName));
            return (await QueryAsync<TRole>(filter: qry, cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            return UpdateAsync(PartitionKey, ConvertToString(role.Id), role, cancellationToken);
        }
    }
}
