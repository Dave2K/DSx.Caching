using DSx.Caching.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni base per un provider di caching
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni di configurazione</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default);

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni di configurazione</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato tipizzato dell'operazione</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default);

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave da impostare</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di configurazione</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken ct = default);

        /// <summary>
        /// Rimuove un valore dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken ct = default);

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken ct = default);
    }
}