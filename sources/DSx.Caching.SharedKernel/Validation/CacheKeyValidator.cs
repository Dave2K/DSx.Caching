using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Validation
{
    /// <summary>
    /// Servizio per la validazione delle chiavi di cache
    /// </summary>
    public partial class CacheKeyValidator : ICacheKeyValidator
    {
        [GeneratedRegex(@"^[\w\-]{1,128}$", RegexOptions.Compiled)]
        private static partial Regex KeyRegex();

        /// <inheritdoc/>
        public void Validate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La chiave non può essere vuota");

            if (!KeyRegex().IsMatch(key))
                throw new ArgumentException($"Formato chiave non valido: {key}");
        }

        /// <inheritdoc/>
        public string NormalizeKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                return string.Empty;

            var cleaned = rawKey.Trim().ToLowerInvariant();
            return Regex.Replace(cleaned, @"[^\w\-]", "-");
        }
    }
}