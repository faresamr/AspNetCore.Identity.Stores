using System;
using System.Text;

namespace AspNetCore.Identity.Stores.Shared.Extensions
{
    internal static class ByteArrayExtensions
    {
        public static byte[] ConvertToByteArray<T>(this T value)
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
        public static object ConvertFromByteArray(this byte[] array, Type type)
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
