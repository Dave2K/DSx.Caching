using DSx.Caching.Abstractions.Interfaces;
using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Keys
{
    /// <summary>
    /// Genera chiavi di cache normalizzate e valide secondo specifiche
    /// </summary>
    public sealed partial class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private const int MaxKeyLength = 128;

        /// <summary>
        /// Genera una chiave univoca combinando il baseKey con i parametri
        /// </summary>
        /// <param name="baseKey">Identificativo base della risorsa</param>
        /// <param name="parameters">Parametri aggiuntivi per l'unicità</param>
        /// <returns>Chiave normalizzata e validata</returns>
        public string GenerateKey(string baseKey, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(baseKey))
                throw new ArgumentNullException(nameof(baseKey));

            var combined = parameters.Length > 0
                ? $"{baseKey}_{string.Join("_", parameters)}"
                : baseKey;

            return NormalizeKey(combined);
        }

        /// <summary>
        /// Normalizza una chiave rimuovendo caratteri non validi e applicando formattazione
        /// </summary>
        /// <param name="rawKey">Chiave grezza da processare</param>
        /// <returns>Chiave conforme alle specifiche</returns>
        public string NormalizeKey(string rawKey)
        {
            var cleaned = NormalizationRegex().Replace(rawKey.Trim(), "-");
            var normalized = cleaned.Length > MaxKeyLength
                ? cleaned[..MaxKeyLength]
                : cleaned;

            return normalized.ToLowerInvariant();
        }

        [GeneratedRegex(@"[^\w\-]", RegexOptions.Compiled)]
        private static partial Regex NormalizationRegex();
    }
}
