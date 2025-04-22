using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using System;
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
        /// Evento scatenato prima di un'operazione sulla cache
        /// </summary>
        event EventHandler<CacheEventArgs> BeforeOperation;

        /// <summary>
        /// Evento scatenato dopo un'operazione sulla cache
        /// </summary>
        event EventHandler<CacheEventArgs> AfterOperation;

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene un elemento dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento</typeparam>
        /// <param name="key">Chiave dell'elemento</param>
        /// <param name="options">Opzioni aggiuntive</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione con valore</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene i metadati di una voce della cache
        /// </summary>
        /// <param name="key">Chiave dell'elemento</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Descrittore della voce o null</returns>
        Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Imposta un elemento nella cache
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento</typeparam>
        /// <param name="key">Chiave dell'elemento</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni aggiuntive</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave dell'elemento</param>
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

        /// <summary>
        /// Evento sollevato quando un'operazione viene differita
        /// </summary>
        /// <remarks>
        /// Funzionalità Enterprise: Gestione di carichi elevati con backpressure
        /// </remarks>
        event EventHandler<OperationDeferredEventArgs> OperationDeferred;
    }
}