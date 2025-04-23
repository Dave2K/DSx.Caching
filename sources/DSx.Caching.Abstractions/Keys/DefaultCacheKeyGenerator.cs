using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DSx.Caching.Abstractions.Keys
{
    /// <summary>
    /// Genera e normalizza chiavi di cache secondo specifiche standard.
    /// Implementa politiche di validazione e sanificazione delle chiavi.
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private static readonly Regex InvalidCharsRegex = new(@"[^a-zA-Z0-9_-]", RegexOptions.Compiled);
        private const int MaxKeyLength = 256;

        /// <summary>
        /// Genera una chiave di cache combinando una chiave base con parametri opzionali
        /// </summary>
        public string GenerateKey(string baseKey, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(baseKey))
                throw new ArgumentNullException(nameof(baseKey));

            var sb = new StringBuilder(baseKey);

            foreach (var param in parameters)
            {
                sb.Append('_');
                sb.Append(param?.ToString()?.Trim() ?? "NULL");
            }

            return NormalizeKey(sb.ToString());
        }

        /// <summary>
        /// Normalizza una chiave rimuovendo caratteri non validi e applicando lowercase
        /// </summary>
        public string NormalizeKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                throw new ArgumentException("La chiave non può essere vuota", nameof(rawKey));

            // 1. Trim degli spazi
            var trimmed = rawKey.Trim();

            // 2. Sostituzione caratteri non validi
            var cleaned = InvalidCharsRegex.Replace(trimmed, "-");

            // 3. Conversione a lowercase
            var lowerCase = cleaned.ToLowerInvariant();

            // 4. Limite lunghezza
            return lowerCase.Length > MaxKeyLength ?
                lowerCase.Substring(0, MaxKeyLength) :
                lowerCase;
        }
    }
}
