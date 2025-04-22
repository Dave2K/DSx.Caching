using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DSx.Caching.Abstractions.Keys
{
    /// <summary>
    /// Implementazione di default per la generazione di chiavi di cache.
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private static readonly Regex InvalidCharsRegex = new(@"[^a-zA-Z0-9_-]", RegexOptions.Compiled);

        /// <inheritdoc/>
        public string GenerateKey(string baseKey, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(baseKey))
                throw new ArgumentNullException(nameof(baseKey));

            var sb = new StringBuilder(baseKey);

            foreach (var param in parameters)
            {
                sb.Append('_');
                sb.Append(param?.ToString() ?? "NULL");
            }

            return NormalizeKey(sb.ToString());
        }

        /// <inheritdoc/>
        public string NormalizeKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                throw new ArgumentException("La chiave non può essere vuota", nameof(rawKey));

            return InvalidCharsRegex.Replace(rawKey, "-");
        }
    }
}