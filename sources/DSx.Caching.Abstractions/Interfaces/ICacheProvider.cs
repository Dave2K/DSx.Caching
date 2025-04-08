using DSx.Caching.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Interfaccia base per i provider di cache
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni aggiuntive</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Esito dell'operazione</returns>
        /// <example>
        /// <code>
        /// var result = await cache.ExistsAsync("mia_chiave");
        /// if (result.Status == CacheOperationStatus.Success) {...}
        /// </code>
        /// </example>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        // Altri metodi con commenti analoghi...
    }
}