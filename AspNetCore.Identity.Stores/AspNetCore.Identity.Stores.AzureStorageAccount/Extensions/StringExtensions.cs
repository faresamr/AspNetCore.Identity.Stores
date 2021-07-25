using System;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions
{
    internal static class StringExtensions
    {
        public static string GetHashString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(value);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash);
            }
        }
    }
}
