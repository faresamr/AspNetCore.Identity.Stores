using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions
{
    internal static class TableEntityExtensions
    {
        public static void CopyTo<T>(this TableEntity tableEntity, ref T obj) where T : class
        {
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead))
            {
                if (tableEntity.TryGetValue(property.Name, out object value))
                {
                    property.SetValue(obj, value);
                }
            }
        }
        public static T ConvertTo<T>(this TableEntity tableEntity) where T : class, new()
        {
            T obj = new();
            tableEntity.CopyTo(ref obj);
            return obj;
        }

        public static TableEntity ToTableEntity<T>(this T obj, string partitionKey, string rowKey)
            where T : class
        {
            var entity = new TableEntity(partitionKey, rowKey);
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead))
                entity.Add(property.Name, property.GetValue(obj));
            return entity;
        }
    }
}
