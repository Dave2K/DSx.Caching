using DSx.Caching.Abstractions.Validators;
using System;
using System.Text.RegularExpressions;

namespace DSx.Caching.Core.Validators
{
    /// <summary>
    /// Implementazione concreta del validatore per le chiavi di cache
    /// </summary>
    public class CacheKeyValidator : ICacheKeyValidator
    {
        private static readonly Regex _keyRegex = new(
            @"^[a-zA-Z0-9_-]{1,128}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Esegue la validazione completa di una chiave secondo le specifiche
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="ArgumentNullException">Chiave null</exception>
        /// <exception cref="ArgumentException">Formato chiave non valido</exception>
        public void Validate(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key), "La chiave non può essere null");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Chiave vuota o contenente solo spazi", nameof(key));

            if (!_keyRegex.IsMatch(key))
            {
                throw new ArgumentException(
                    $"Formato chiave non valido: '{key}'. Caratteri permessi: A-Z, a-z, 0-9, -, _",
                    nameof(key));
            }
        }
    }
}