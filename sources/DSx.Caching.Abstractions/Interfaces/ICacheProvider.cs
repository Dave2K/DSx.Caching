using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni di base per un provider di cache.
    /// </summary>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache.
        /// </summary>
        event EventHandler<CacheEventArgs> BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache.
        /// </summary>
        event EventHandler<CacheEventArgs> AfterOperation;

        /// <summary>
        /// Recupera un valore dalla cache.
        /// </summary>
        /// <typeparam name="T">Tipo del valore memorizzato.</typeparam>
        /// <param name="key">Chiave associata al valore.</param>
        /// <param name="options">Opzioni aggiuntive per il recupero.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione, incluso lo stato e il valore.</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache.
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare.</typeparam>
        /// <param name="key">Chiave associata al valore.</param>
        /// <param name="value">Valore da memorizzare.</param>
        /// <param name="options">Opzioni di scadenza e priorità.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una voce dalla cache.
        /// </summary>
        /// <param name="key">Chiave della voce da rimuovere.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente la cache.
        /// </summary>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera i metadati di una voce nella cache.
        /// </summary>
        /// <param name="key">Chiave della voce.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Metadati della voce o <c>null</c> se non trovata.</returns>
        Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Tipo di operazione eseguita sulla cache.
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary> Operazione di lettura. </summary>
        Get,
        /// <summary> Operazione di scrittura. </summary>
        Set,
        /// <summary> Operazione di rimozione. </summary>
        Remove,
        /// <summary> Operazione di svuotamento totale. </summary>
        ClearAll
    }
}