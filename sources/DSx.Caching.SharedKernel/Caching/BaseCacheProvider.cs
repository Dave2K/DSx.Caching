using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Fornisce un'implementazione base per i provider di cache con gestione completa del ciclo di vita
    /// </summary>
    public abstract class BaseCacheProvider(ILogger logger) : ICacheProvider, IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Flag che indica se l'istanza è stata disposed
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// Logger per la registrazione delle attività
        /// </summary>
        protected ILogger Logger { get; } = logger;

        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache
        /// </summary>
        public abstract event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache
        /// </summary>
        public abstract event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Recupera un elemento dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da recuperare</typeparam>
        /// <param name="key">Chiave identificativa dell'elemento</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un elemento nella cache
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da memorizzare</typeparam>
        /// <param name="key">Chiave identificativa</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di memorizzazione</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave dell'elemento da rimuovere</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera i metadati di un elemento nella cache
        /// </summary>
        /// <param name="key">Chiave dell'elemento</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Descrizione dell'elemento</returns>
        public abstract Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Esito della verifica</returns>
        public abstract Task<CacheOperationResult<bool>> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica lo stato di disposizione dell'oggetto
        /// </summary>
        /// <exception cref="ObjectDisposedException">Se l'oggetto è già stato disposed</exception>
        protected void CheckDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

        /// <summary>
        /// Esegue la pulizia delle risorse gestite
        /// </summary>
        /// <param name="disposing">Indica se la disposizione è esplicita</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Pulizia risorse gestite
            }
            _disposed = true;
        }

        /// <summary>
        /// Esegue la pulizia asincrona delle risorse
        /// </summary>
        protected virtual ValueTask DisposeAsyncCore()
        {
            Dispose(false);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Implementazione di IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione di IAsyncDisposable
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}
