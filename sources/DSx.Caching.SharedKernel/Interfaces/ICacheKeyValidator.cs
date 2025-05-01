using DSx.Caching.SharedKernel.Exceptions;

namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Definisce il contratto per i validatori di chiavi della cache
    /// </summary>
    public interface ICacheKeyValidator
    {
        /// <summary>
        /// Verifica la validità di una chiave secondo le regole implementate
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <exception cref="CacheKeyException">
        /// Sollevata in caso di violazione delle regole di validazione
        /// </exception>
        void Validate(string key);

        /// <summary>
        /// Trasforma una chiave grezza in un formato standardizzato
        /// </summary>
        /// <param name="key">Chiave originale da normalizzare</param>
        /// <returns>Chiave normalizzata pronta per l'uso</returns>
        /// <remarks>
        /// Garantisce la compatibilità con diversi sistemi di caching
        /// </remarks>
        string NormalizeKey(string key);
    }
}
