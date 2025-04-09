using System;

namespace DSx.Caching.Abstractions.Validators
{
    /// <summary>
    /// Definisce il contratto per la validazione delle chiavi utilizzate nella cache
    /// </summary>
    public interface ICacheKeyValidator
    {
        /// <summary>
        /// Esegue la validazione completa di una chiave per la cache
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="ArgumentNullException">
        /// Sollevata quando la chiave è null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Sollevata quando la chiave è vuota, contiene solo spazi o caratteri non permessi
        /// </exception>
        void Validate(string key);
    }
}