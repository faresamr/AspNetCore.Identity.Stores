using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.Identity.Stores.Shared.Extensions
{
    internal static class StringExtensions
    {
        public static string GetHashString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string rawKeyData = value;
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawKeyData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return string.Join(string.Empty, hashBytes.Select(i => i.ToString("X2")));
        }
    }
}
