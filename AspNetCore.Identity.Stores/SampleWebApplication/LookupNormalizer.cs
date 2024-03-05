﻿using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace SampleWebApplication;

public class LookupNormalizer : ILookupNormalizer
{
    public string? NormalizeEmail(string? email) => GetHashString(email?.ToUpperInvariant()).Replace("-", string.Empty);

    public string? NormalizeName(string? name) => GetHashString(name?.ToUpperInvariant()).Replace("-", string.Empty);

    private static string GetHashString(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        byte[] textData = Encoding.UTF8.GetBytes(value);
        byte[] hash = SHA384.HashData(textData);
        return BitConverter.ToString(hash);
    }
}