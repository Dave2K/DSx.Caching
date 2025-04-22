using DSx.Caching.SharedKernel.Validation;
using System.Text.RegularExpressions;

namespace DSx.Caching.SharedKernel.Validation
{
    /// <summary>
    /// Valida e normalizza le chiavi della cache
    /// </summary>
    public class CacheKeyValidator : ICacheKeyValidator
    {
        private readonly Regex _validationRegex;
        private readonly Regex _normalizationRegex;

        /// <summary>
        /// Inizializza un nuovo validatore con regex personalizzate
        /// </summary>
        /// <param name="validationPattern">Pattern per la validazione</param>
        /// <param name="normalizationPattern">Pattern per la normalizzazione</param>
        public CacheKeyValidator(
            string validationPattern = @"^[\w\-]{1,128}$",
            string normalizationPattern = @"[^\w\-]")
        {
            _validationRegex = new Regex(validationPattern, RegexOptions.Compiled);
            _normalizationRegex = new Regex(normalizationPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Verifica la validità di una chiave
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="ArgumentException">Chiave non valida</exception>
        public void Validate(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || !_validationRegex.IsMatch(key))
            {
                throw new ArgumentException($"Chiave non valida: {key}");
            }
        }

        /// <summary>
        /// Normalizza una chiave secondo le regole definite
        /// </summary>
        /// <param name="rawKey">Chiave originale</param>
        /// <returns>Chiave normalizzata</returns>
        public string NormalizeKey(string rawKey)
        {
            var cleaned = _normalizationRegex.Replace(rawKey.Trim(), "-");
            return cleaned.Length > 128 ? cleaned[..128] : cleaned.ToLowerInvariant();
        }
    }
}