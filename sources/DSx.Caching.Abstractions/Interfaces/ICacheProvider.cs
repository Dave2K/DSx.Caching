using DSx.Caching.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni base per un provider di cache
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni di cache</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave di cache</param>
        /// <param name="options">Opzioni di cache</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave di cache</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di cache</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token di cancellazione</param>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);
    }
}