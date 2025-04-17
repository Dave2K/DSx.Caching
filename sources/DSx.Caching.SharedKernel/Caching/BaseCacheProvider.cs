using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Fornisce un'implementazione base per i provider di cache con funzionalità comuni
    /// </summary>
    public abstract class BaseCacheProvider : IDisposable, IAsyncDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Istanza del logger per la registrazione delle operazioni
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Inizializza una nuova istanza della classe BaseCacheProvider
        /// </summary>
        /// <param name="logger">Istanza del logger da utilizzare</param>
        /// <exception cref="ArgumentNullException">Generato quando il logger è null</exception>
        protected BaseCacheProvider(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Rilascia tutte le risorse utilizzate dal provider
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rilascia tutte le risorse in modo asincrono
        /// </summary>
        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Verifica se l'istanza è stata disposed
        /// </summary>
        protected void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Pulizia delle risorse
        /// </summary>
        /// <param name="disposing">True se chiamato da Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Pulizia risorse gestite
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Pulizia asincrona delle risorse
        /// </summary>
        protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni della cache (opzionali)</param>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default
        );

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore memorizzato</typeparam>
        /// <param name="key">Chiave della cache</param>
        /// <param name="options">Opzioni della cache (opzionali)</param>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato contenente il valore o lo stato dell'operazione</returns>
        public abstract Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default
        );

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave della cache</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni della cache (opzionali)</param>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken ct = default
        );

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken ct = default
        );

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public abstract Task<CacheOperationResult> ClearAllAsync(
            CancellationToken ct = default
        );
    }
}