// File: sources/DSx.Caching.Abstractions/Interfaces/ICacheProvider.cs
using DSx.Caching.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni base per un provider di caching
    /// </summary>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione con stato</returns>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Risultato con valore e stato dell'operazione</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave da impostare</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di scadenza e comportamento</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);
    }
}