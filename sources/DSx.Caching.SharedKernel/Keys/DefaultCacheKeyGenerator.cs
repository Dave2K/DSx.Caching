using DSx.Caching.Abstractions.Keys;
using System;
using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Keys
{
    /// <summary>
    /// Implementazione predefinita per la generazione di chiavi di cache
    /// </summary>
    public partial class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        [GeneratedRegex(@"^[\w\-]{1,128}$", RegexOptions.Compiled)]
        private static partial Regex KeyRegex();

        [GeneratedRegex(@"[^\w\-]", RegexOptions.Compiled)]
        private static partial Regex InvalidCharsRegex();

        /// <summary>
        /// Genera una chiave di cache normalizzata
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Se baseKey è null o vuoto</exception>
        public string GenerateKey(string baseKey, object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(baseKey))
                throw new ArgumentNullException(nameof(baseKey));

            return $"{NormalizeKey(baseKey)}_{string.Join("_", parameters)}";
        }

        /// <summary>
        /// Normalizza una chiave secondo le specifiche
        /// </summary>
        /// <inheritdoc/>
        public string NormalizeKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                return string.Empty;

            var cleaned = InvalidCharsRegex().Replace(rawKey.Trim(), "-");
            return cleaned.ToLowerInvariant();
        }
    }
}