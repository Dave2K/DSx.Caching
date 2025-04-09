// File: sources/DSx.Caching.Providers.Memory/MemoryCacheProvider.cs
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Provider di caching in memoria utilizzando IMemoryCache
    /// </summary>
    public sealed class MemoryCacheProvider : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheProvider> _logger;
        private readonly ICacheKeyValidator _keyValidator;
        private bool _disposed;

        /// <summary>
        /// Costruttore principale
        /// </summary>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger,
            ICacheKeyValidator keyValidator)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                return new CacheOperationResult
                {
                    Status = _cache.TryGetValue(key, out _) ?
                        CacheOperationStatus.Success :
                        CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore verifica esistenza chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                return _cache.TryGetValue(key, out T? value)
                    ? new CacheOperationResult<T> { Status = CacheOperationStatus.Success, Value = value! }
                    : new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore recupero valore: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                var cacheEntryOptions = new MemoryCacheEntryOptions();

                if (options != null)
                {
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;
                    cacheEntryOptions.SlidingExpiration = options.SlidingExpiration;
                }

                _cache.Set(key, value, cacheEntryOptions);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore salvataggio valore: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                _cache.Remove(key);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore rimozione chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore pulizia cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_cache is IDisposable disposableCache)
                    disposableCache.Dispose();

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_cache is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else if (_cache is IDisposable disposable)
                disposable.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}