using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Azure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories
{
    internal abstract class TableStorage
    {
        internal const string IdentityTable = "Identity";
        private readonly IDataProtector dataProtector;
        private readonly Container container;

        public TableStorage(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            dataProtector = dataProtectionProvider.CreateProtector(options.Value.GetContainerId());

            CosmosClient cosmosClient = new(options.Value.GetConnectionString());
            Database database = cosmosClient.GetDatabase(options.Value.GetDatabaseId());
            container = database.GetContainer(options.Value.GetContainerId());
        }
    
        protected static string ConvertToString<T>(T value) where T : IEquatable<T> => Convert.ToString(value);

        protected async Task<IdentityResult> AddAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                _ = await container.UpsertItemAsync(ToTableEntity(entity, partitionKey, rowKey), cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (CosmosException ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.StatusCode.ToString(), Description = ex.Message });
            }
        }

        protected async Task<IdentityResult> DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _ = await container.DeleteItemAsync<TableEntity>(rowKey, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (CosmosException ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.StatusCode.ToString(), Description = ex.Message });
            }
        }

        protected async Task<IList<T>> QueryAsync<T>(string filter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            try
            {
                QueryDefinition queryDefinition = new(filter);
                FeedIterator<TableEntity> queryResultSetIterator = container.GetItemQueryIterator<TableEntity>(queryDefinition);

                List<T> entities = new();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<TableEntity> currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
                    foreach (TableEntity entity in currentResultSet)
                    {
                        entities.Add(ConvertTo<T>(entity));
                    }
                }
                return entities;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Enumerable.Empty<T>().ToList();;
            }
        }
        protected IEnumerable<T> Query<T>(string filter = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            return QueryAsync<T>(filter, cancellationToken).Result;
        }

        protected async Task<T> QueryAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, new()
        {
            try
            {
                var response = await container.ReadItemAsync<TableEntity>(rowKey, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                if (response.StatusCode== System.Net.HttpStatusCode.OK)
                    return ConvertTo<T>(response.Resource);
                else
                    return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        protected static string BuildQuery(string partitionKey, params KeyValuePair<string, object>[] filters)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("SELECT * FROM c WHERE c.").Append(TableEntity.PartitionKey).Append('=').Append('\'').Append(partitionKey).Append('\'');
            if (filters.Any())
            {
                foreach (var keyValue in filters)
                {
                    stringBuilder.Append(" AND c.").Append(keyValue.Key).Append('=').Append('\'').Append(keyValue.Value).Append('\'');
                }
            }
            return stringBuilder.ToString();
        }

        public async Task<IdentityResult> UpdateAsync<T>(string partitionKey, string rowKey, T entity, CancellationToken cancellationToken) where T : class
        {
            try
            {
                var tableEntity = ToTableEntity(entity, partitionKey, rowKey);
                _ = await container.UpsertItemAsync(tableEntity, cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (CosmosException ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.StatusCode.ToString(), Description = ex.Message });
            }
        }
        private T ConvertTo<T>(TableEntity tableEntity) where T : class, new()
        {
            T obj = new();
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead && i.Name != TableEntity.PartitionKey && i.Name != TableEntity.Id))
            {
                if (tableEntity.TryGetValue(property.Name, out object value))
                {
                    if (property.GetCustomAttribute<ProtectedPersonalDataAttribute>() is ProtectedPersonalDataAttribute)
                    {
                        string strData = value as string;
                        var data = Convert.FromBase64CharArray(strData.ToArray(), 0, strData.Length);
                        property.SetValue(obj, dataProtector.Unprotect(data).ConvertFromByteArray(property.PropertyType));
                    }
                    else
                    {
                        if (property.PropertyType == value.GetType())
                            property.SetValue(obj, value);
                        else
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }
            return obj;
        }

        private TableEntity ToTableEntity<T>(T obj, string partitionKey, string rowKey)
            where T : class
        {
            var entity = new TableEntity(partitionKey, rowKey);
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead && i.Name != TableEntity.PartitionKey && i.Name != TableEntity.Id))
            {
                if (property.GetValue(obj) is object propertyValue)
                {
                    if (property.GetCustomAttribute<ProtectedPersonalDataAttribute>() is ProtectedPersonalDataAttribute)
                    {
                        entity.Add(property.Name, dataProtector.Protect(propertyValue.ConvertToByteArray()));
                    }
                    else
                    {
                        entity.Add(property.Name, propertyValue);
                    }
                }
            }

            return entity;
        }
    }
    internal class TableEntity : Dictionary<string, object>
    {
        public const string PartitionKey = "PartitionKey";
        public const string Id = "id";

        public TableEntity()
        {

        }
        public TableEntity(string partitionKey, string id)
        {
            Add(PartitionKey, partitionKey);
            Add(Id, id);
        }

    }
}
