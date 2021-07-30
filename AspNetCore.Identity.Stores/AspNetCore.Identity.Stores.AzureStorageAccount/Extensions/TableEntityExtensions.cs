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
                        property.SetValue(obj, value);
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
                object value;
                if (property.GetCustomAttribute<ProtectedPersonalDataAttribute>() is ProtectedPersonalDataAttribute)
                {
                    value = dataProtector.Protect(property.GetValue(obj).ConvertToByteArray());
                }
                else
                {
                    value = property.GetValue(obj);
                }
                entity.Add(property.Name, value);
            }

            return entity;
        }

        private static byte[] ConvertToByteArray<T>(this T value)
        {
            return value switch
            {
                int intValue => BitConverter.GetBytes(intValue),
                uint uintValue => BitConverter.GetBytes(uintValue),
                double doubleValue => BitConverter.GetBytes(doubleValue),
                ushort ushortValue => BitConverter.GetBytes(ushortValue),
                float floatValue => BitConverter.GetBytes(floatValue),
                long longValue => BitConverter.GetBytes(longValue),
                ulong ulongValue => BitConverter.GetBytes(ulongValue),
                short shortValue => BitConverter.GetBytes(shortValue),
                char charValue => BitConverter.GetBytes(charValue),
                bool boolValue => BitConverter.GetBytes(boolValue),
                string stringValue => Encoding.UTF8.GetBytes(stringValue),
                _ when typeof(T).IsEnum => ConvertToByteArray(value.ToString()),
                _ => throw new NotSupportedException()
            };
        }
        private static object ConvertFromByteArray(this byte[] array, Type type)
        {
            if (type == typeof(int)) return BitConverter.ToInt32(array);
            else if (type == typeof(uint)) return BitConverter.ToUInt32(array);
            else if (type == typeof(double)) return BitConverter.ToDouble(array);
            else if (type == typeof(ushort)) return BitConverter.ToUInt16(array);
            else if (type == typeof(float)) return BitConverter.ToSingle(array);
            else if (type == typeof(long)) return BitConverter.ToInt64(array);
            else if (type == typeof(ulong)) return BitConverter.ToUInt64(array);
            else if (type == typeof(short)) return BitConverter.ToInt16(array);
            else if (type == typeof(char)) return BitConverter.ToChar(array);
            else if (type == typeof(bool)) return BitConverter.ToBoolean(array);
            else if (type == typeof(string)) return Encoding.UTF8.GetString(array);
            else if (type.IsEnum) return Enum.Parse(type, ConvertFromByteArray(array, typeof(string)) as string);
            else throw new NotSupportedException();
        }
    }
}
