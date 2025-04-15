using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Classe base astratta per l'implementazione dei provider di cache
    /// </summary>
    /// <remarks>
    /// Fornisce funzionalità base per la gestione del ciclo di vita e il tracking delle operazioni
    /// </remarks>
    public abstract class BaseCacheProvider(ILogger logger) : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private bool _disposed;

        /// <inheritdoc/>
        public event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <inheritdoc/>
        public event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Logger per la tracciatura delle attività
        /// </summary>
        protected ILogger Logger { get; } = logger;

        /// <summary>
        /// Verifica lo stato di disposed dell'oggetto
        /// </summary>
        /// <exception cref="ObjectDisposedException">Se l'istanza è stata eliminata</exception>
        protected void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

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
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione del pattern Dispose
        /// </summary>
        /// <param name="disposing">Indica se è in corso una dispose esplicita</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Cleanup risorse gestite
            }
            _disposed = true;
        }

        /// <summary>
        /// Implementazione asincrona del pattern Dispose
        /// </summary>
        protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

        /// <summary>
        /// Gestione evento pre-operazione
        /// </summary>
        /// <param name="e">Argomenti dell'evento</param>
        /// <exception cref="ArgumentNullException">Se gli argomenti sono null</exception>
        protected virtual void OnBeforeOperation(CacheEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            BeforeOperation?.Invoke(this, e);
        }

        /// <summary>
        /// Gestione evento post-operazione
        /// </summary>
        /// <param name="e">Argomenti dell'evento</param>
        /// <exception cref="ArgumentNullException">Se gli argomenti sono null</exception>
        protected virtual void OnAfterOperation(CacheEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            AfterOperation?.Invoke(this, e);
        }
    }
}