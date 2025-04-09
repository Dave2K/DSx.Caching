using DSx.Caching.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Interfaccia base per tutti i provider di cache con gestione unificata degli errori
    /// </summary>
    /// <remarks>
    /// Definisce il contratto comune per tutte le implementazioni di cache
    /// </remarks>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Verifica se una chiave esiste nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni di scadenza del cache</param>
        /// <param name="cancellationToken">Token per cancellare l'operazione</param>
        /// <returns>Task che restituisce il risultato dell'operazione</returns>
        /// <exception cref="ArgumentNullException">Se la chiave è null o vuota</exception>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni di scadenza del cache</param>
        /// <param name="cancellationToken">Token per cancellare l'operazione</param>
        /// <returns>Task che restituisce il valore e risultato dell'operazione</returns>
        /// <exception cref="ArgumentNullException">Se la chiave è null o vuota</exception>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave da usare</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di scadenza del cache</param>
        /// <param name="cancellationToken">Token per cancellare l'operazione</param>
        /// <returns>Task che restituisce il risultato dell'operazione</returns>
        /// <exception cref="ArgumentNullException">Se la chiave o il valore sono null</exception>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token per cancellare l'operazione</param>
        /// <returns>Task che restituisce il risultato dell'operazione</returns>
        /// <exception cref="ArgumentNullException">Se la chiave è null o vuota</exception>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente il contenuto della cache
        /// </summary>
        /// <param name="cancellationToken">Token per cancellare l'operazione</param>
        /// <returns>Task che restituisce il risultato dell'operazione</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);
    }
}