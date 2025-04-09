using System;
using System.Text.RegularExpressions;

namespace DSx.Caching.Abstractions.Validators
{
    /// <summary>
    /// Validatore per le chiavi della cache
    /// </summary>
    public static class CacheKeyValidator
    {
        private static readonly Regex _keyRegex = new(@"^[a-zA-Z0-9_-]{1,128}$", RegexOptions.Compiled);

        /// <summary>
        /// Verifica la validità di una chiave di cache
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="ArgumentException">Se la chiave non è valida</exception>
        public static void ThrowIfInvalid(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key), "La chiave non può essere null");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("La chiave non può essere vuota o contenere solo spazi", nameof(key));

            if (!_keyRegex.IsMatch(key))
            {
                throw new ArgumentException(
                    $"Formato chiave non valido: {key}. Caratteri permessi: A-Z, a-z, 0-9, -, _",
                    nameof(key)
                );
            }
        }
    }
}