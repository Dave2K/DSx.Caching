using DSx.Caching.SharedKernel.Interfaces;
using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Validation
{
    /// <summary>
    /// Fornisce servizi per la validazione e normalizzazione delle chiavi di cache
    /// </summary>
    public class CacheKeyValidator : ICacheKeyValidator
    {
        private const int MaxKeyLength = 256;
        private static readonly Regex InvalidCharactersRegex = new(
            pattern: "[^a-z0-9_-]",
            options: RegexOptions.Compiled
        );
        private static readonly Regex MultiSeparatorsRegex = new(
            pattern: "[-_]{2,}",
            options: RegexOptions.Compiled
        );

        /// <summary>
        /// Normalizza una chiave secondo le regole di formattazione
        /// </summary>
        /// <param name="key">Chiave da normalizzare</param>
        /// <returns>Chiave normalizzata</returns>
        /// <exception cref="ArgumentException">Se la chiave è vuota o contiene solo spazi</exception>
        public string NormalizeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La chiave non può essere vuota", nameof(key));

            // Passaggi di normalizzazione
            var normalized = key
                .ToLowerInvariant() // Converti in minuscolo
                .Trim() // Rimuovi spazi all'inizio/fine
                .Replace("_", "-"); // Sostituisci underscore con trattini

            // Sostituisci caratteri non validi
            normalized = InvalidCharactersRegex.Replace(normalized, "-");

            // Unisci separatori multipli
            normalized = MultiSeparatorsRegex.Replace(normalized, "-");

            // Troncamento e pulizia finale
            normalized = normalized.Trim('-');
            if (normalized.Length > MaxKeyLength)
                normalized = normalized.Substring(0, MaxKeyLength).Trim('-');

            return normalized;
        }

        /// <summary>
        /// Valida una chiave contro le regole di formattazione
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="InvalidCacheKeyException">Sollevata per chiavi non valide</exception>
        public void Validate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidCacheKeyException("Chiave vuota", "EMPTY_KEY", key);

            var normalized = NormalizeKey(key);

            if (normalized != key)
                throw new InvalidCacheKeyException(
                    "Formato chiave non valido",
                    "INVALID_FORMAT",
                    key
                );

            if (normalized.Length > MaxKeyLength)
                throw new InvalidCacheKeyException(
                    $"Lunghezza massima consentita: {MaxKeyLength} caratteri",
                    "MAX_LENGTH_EXCEEDED",
                    key
                );
        }
    }
}
