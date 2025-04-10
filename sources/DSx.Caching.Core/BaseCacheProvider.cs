using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Core
{
    /// <summary>
    /// Fornisce un'implementazione base per i provider di cache
    /// </summary>
    /// <remarks>
    /// Implementa il pattern Dispose e fornisce metodi astratti per le operazioni di base
    /// </remarks>
    public abstract class BaseCacheProvider(ILogger logger) : ICacheProvider, IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Indica se l'oggetto è stato già eliminato
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// Logger per la registrazione delle attività
        /// </summary>
        protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Rilascia le risorse gestite e non gestite
        /// </summary>
        /// <param name="disposing">True per rilasciare le risorse gestite</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Rilascio risorse gestite
            }
            _disposed = true;
        }
    }
}