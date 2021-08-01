using AspNetCore.Identity.Stores.Shared.Extensions;
using Azure.Data.Tables;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions
{
    internal static class TableEntityExtensions
    {
        public static T ConvertTo<T>(this TableEntity tableEntity, IDataProtector dataProtector) where T : class, new()
        {
            T obj = new();
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead))
            {
                if (tableEntity.TryGetValue(property.Name, out object value))
                {
                    if (property.GetCustomAttribute<ProtectedPersonalDataAttribute>() is ProtectedPersonalDataAttribute)
                    {
                        property.SetValue(obj, dataProtector.Unprotect(value as byte[]).ConvertFromByteArray(property.PropertyType));
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

        public static TableEntity ToTableEntity<T>(this T obj, string partitionKey, string rowKey, IDataProtector dataProtector)
            where T : class
        {
            var entity = new TableEntity(partitionKey, rowKey);
            foreach (var property in typeof(T).GetProperties().Where(i => i.CanWrite && i.CanRead))
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
}
