using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Implementazione di un provider di cache in memoria
    /// </summary>
    public sealed class MemoryCacheProvider : BaseCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheKeyValidator _keyValidator;

        /// <summary>
        /// Inizializza una nuova istanza del provider MemoryCache
        /// </summary>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger,
            ICacheKeyValidator keyValidator)
            : base(logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        }

        /// <inheritdoc/>
        public override Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                return Task.FromResult(new CacheOperationResult
                {
                    Status = _cache.TryGetValue(key, out _) ?
                        CacheOperationStatus.Success :
                        CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public override Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache.TryGetValue(key, out T? value) && value is not null)
                {
                    return Task.FromResult(new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = value
                    });
                }

                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public override Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var cacheEntryOptions = new MemoryCacheEntryOptions();

                if (options?.AbsoluteExpiration != null)
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;

                if (options?.SlidingExpiration != null)
                    cacheEntryOptions.SlidingExpiration = options.SlidingExpiration;

                _cache.Set(key, value, cacheEntryOptions);
                return Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public override Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                _cache.Remove(key);
                return Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public override Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento completo della cache");
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cache?.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}