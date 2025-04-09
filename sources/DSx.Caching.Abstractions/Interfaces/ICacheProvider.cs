using DSx.Caching.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Fornisce operazioni di caching con supporto asincrono
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Verifica l'esistenza di una voce nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni configurabili per la voce</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato con stato di esistenza</returns>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore memorizzato</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni configurabili per la voce</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato con valore se trovato</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave di memorizzazione</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di scadenza e rimozione</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una voce dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);
    }
}