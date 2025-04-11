using DSx.Caching.Abstractions.Models;
using DSx.Caching.Core;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Fornisce un'implementazione di cache in memoria utilizzando IMemoryCache
    /// </summary>
    public sealed class MemoryCacheProvider : BaseCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheKeyValidator _keyValidator;

        /// <summary>
        /// Inizializza una nuova istanza della classe MemoryCacheProvider
        /// </summary>
        /// <param name="cache">Istanza della cache in memoria</param>
        /// <param name="logger">Logger per il tracciamento delle attivit�</param>
        /// <param name="keyValidator">Validatore per le chiavi della cache</param>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger,
            ICacheKeyValidator keyValidator)
            : base(logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                return await Task.FromResult(new CacheOperationResult
                {
                    Status = _cache.TryGetValue(key, out _) ?
                        CacheOperationStatus.Success :
                        CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica della chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <inheritdoc/>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache.TryGetValue(key, out T? value) && value != null)
                {
                    return await Task.FromResult(new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = value
                    });
                }

                return await Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero della chiave {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <inheritdoc/>
        public override async Task<CacheOperationResult> SetAsync<T>(
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
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio della chiave {Key}", key);
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
        /// <inheritdoc/>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                _cache.Remove(key);
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione della chiave {Key}", key);
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
        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rilascia le risorse gestite
        /// </summary>
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