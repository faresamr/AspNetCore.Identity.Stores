using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using AspNetCore.Identity.Stores.Shared.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;

internal abstract class CosmosContainer
{
    private readonly IDataProtector dataProtector;
    private readonly Container container;

    public CosmosContainer(IDataProtectionProvider dataProtectionProvider, IOptions<IdentityStoresOptions> options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        dataProtector = dataProtectionProvider.CreateProtector(options.Value.GetContainerId());

        CosmosClient cosmosClient = options.Value.GetCosmosClient();
        Database database = cosmosClient.GetDatabase(options.Value.GetDatabaseId());
        container = database.GetContainer(options.Value.GetContainerId());
    }

    protected static string ConvertToString<T>(T value) where T : IEquatable<T> => Convert.ToString(value) ?? string.Empty;

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
            _ = await container.DeleteItemAsync<CosmosContainerEntity>(rowKey, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (CosmosException ex)
        {
            return IdentityResult.Failed(new IdentityError { Code = ex.StatusCode.ToString(), Description = ex.Message });
        }
    }
    protected async Task DeleteBulkAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        if (queryDefinition is null)
        {
            throw new ArgumentNullException(nameof(queryDefinition));
        }

        FeedIterator<CosmosContainerEntity> queryResultSetIterator = container.GetItemQueryIterator<CosmosContainerEntity>(queryDefinition);

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<CosmosContainerEntity> currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
            foreach (CosmosContainerEntity entity in currentResultSet)
            {
                string partitionKey = (string)entity[CosmosContainerEntity.PartitionKey];
                string id = (string)entity[CosmosContainerEntity.Id];
                await DeleteAsync(partitionKey, id, cancellationToken);
            }
        }
    }

    protected async Task<IList<T>> QueryAsync<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken = default) where T : class, new()
    {
        try
        {
            FeedIterator<CosmosContainerEntity> queryResultSetIterator = container.GetItemQueryIterator<CosmosContainerEntity>(queryDefinition);

            List<T> entities = new();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CosmosContainerEntity> currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
                foreach (CosmosContainerEntity entity in currentResultSet)
                {
                    entities.Add(ConvertTo<T>(entity));
                }
            }
            return entities;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return Enumerable.Empty<T>().ToList();
        }
    }
    protected IEnumerable<T> Query<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken = default) where T : class, new()
    {
        return QueryAsync<T>(queryDefinition, cancellationToken).Result;
    }

    protected async Task<T?> QueryAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, new()
    {
        try
        {
            var response = await container.ReadItemAsync<CosmosContainerEntity>(rowKey, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
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

    protected static QueryDefinition BuildQuery(string partitionKey, params (string Name, object Value)[] filters)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append("SELECT * FROM c WHERE c.").Append(CosmosContainerEntity.PartitionKey).Append('=').Append("@PartitionKey");
        if (filters.Any())
        {
            foreach (var keyValue in filters)
            {
                stringBuilder.Append(" AND c.").Append(keyValue.Name).Append('=').Append('@').Append(keyValue.Name);
            }
        }
        QueryDefinition queryDefinition = new(stringBuilder.ToString());
        queryDefinition.WithParameter("@PartitionKey", partitionKey);
        if (filters.Any())
        {
            foreach (var keyValue in filters)
            {
                queryDefinition.WithParameter($"@{keyValue.Name}", keyValue.Value);
            }
        }

        return queryDefinition;
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
    private T ConvertTo<T>(CosmosContainerEntity cosmosContainerEntity) where T : class, new()
    {
        T obj = new();
        foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead && i.Name != CosmosContainerEntity.PartitionKey && i.Name != CosmosContainerEntity.Id))
        {
            if (cosmosContainerEntity.TryGetValue(property.Name, out object? value))
            {
                if (property.GetCustomAttribute<ProtectedPersonalDataAttribute>() is not null)
                {
                    string strData = Convert.ToString(value) ?? string.Empty;
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

    private CosmosContainerEntity ToTableEntity<T>(T obj, string partitionKey, string rowKey)
        where T : class
    {
        var entity = new CosmosContainerEntity(partitionKey, rowKey);
        foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead && i.Name != CosmosContainerEntity.PartitionKey && i.Name != CosmosContainerEntity.Id))
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
internal class CosmosContainerEntity : Dictionary<string, object>
{
    public const string PartitionKey = "PartitionKey";
    public const string Id = "id";

    public CosmosContainerEntity()
    {

    }
    public CosmosContainerEntity(string partitionKey, string id)
    {
        Add(PartitionKey, partitionKey);
        Add(Id, id);
    }

}
