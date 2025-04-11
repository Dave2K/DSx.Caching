// File: sources/DSx.Caching.SharedKernel/Validation/CacheKeyValidator.cs
using System;
using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Validation
{
    public class CacheKeyValidator : ICacheKeyValidator
    {
        private static readonly Regex KeyRegex = new Regex(
            @"^[a-zA-Z0-9_-]{1,128}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100)
        );

        public void Validate(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty or whitespace", nameof(key));

            if (!KeyRegex.IsMatch(key))
                throw new ArgumentException($"Invalid key format: '{key}'. Allowed characters: A-Z, a-z, 0-9, -, _");
        }

        public string NormalizeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            return KeyRegex.Replace(key.Trim(), "-")
                .Replace(" ", "-", StringComparison.Ordinal)
                .Trim('-');
        }
    }
}