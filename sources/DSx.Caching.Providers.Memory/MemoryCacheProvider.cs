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
    /// Provider di cache in memoria con gestione completa delle operazioni
    /// </summary>
    public sealed class MemoryCacheProvider : ICacheProvider, IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheProvider> _logger;
        private readonly ICacheKeyValidator _keyValidator;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza del provider
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
        public async Task<CacheOperationResult> ClearAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return await Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la pulizia completa della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                return await Task.FromResult(new CacheOperationResult
                {
                    Status = _cache.TryGetValue(key, out _) ?
                        CacheOperationStatus.Success :
                        CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
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
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                if (_cache.TryGetValue(key, out T? value))
                {
                    return await Task.FromResult(new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = value!
                    });
                }

                return await Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero valore: {Key}", key);
                return new CacheOperationResult<T>
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
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                _cache.Remove(key);
                return await Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return new CacheOperationResult
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
                return await Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio valore: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Gestione della disposizione delle risorse
        /// </summary>
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
    }
}