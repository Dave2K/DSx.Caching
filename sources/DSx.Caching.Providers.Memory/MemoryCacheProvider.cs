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
    /// Implementazione di una cache in memoria
    /// </summary>
    public sealed class MemoryCacheProvider : ICacheProvider, IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheProvider> _logger;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza del provider di cache in memoria
        /// </summary>
        public MemoryCacheProvider(IMemoryCache cache, ILogger<MemoryCacheProvider> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        public async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                var exists = _cache.TryGetValue(key, out _);
                return await Task.FromResult(new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica della chiave '{Key}'", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

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
                _logger.LogError(ex, "Errore durante il recupero della chiave '{Key}'", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

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
                _logger.LogError(ex, "Errore durante il salvataggio della chiave '{Key}'", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                _cache.Remove(key);
                return await Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione della chiave '{Key}'", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                {
                    memoryCache.Compact(1.0);
                }

                return await Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la pulizia della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rilascia le risorse utilizzate
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_cache is IDisposable disposableCache)
                {
                    disposableCache.Dispose();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}