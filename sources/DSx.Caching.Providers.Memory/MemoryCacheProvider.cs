using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Caching;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Fornisce un'implementazione concreta di una cache in memoria basata su <see cref="IMemoryCache"/>
    /// </summary>
    public sealed class MemoryCacheProvider : BaseCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheKeyValidator _keyValidator;

        /// <summary>
        /// Inizializza una nuova istanza del provider di cache in memoria
        /// </summary>
        /// <param name="cache">Istanza della cache in memoria</param>
        /// <param name="logger">Logger per la tracciatura delle operazioni</param>
        /// <param name="keyValidator">Validatore delle chiavi di cache</param>
        /// <exception cref="ArgumentNullException">Se uno dei parametri è null</exception>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger,
            ICacheKeyValidator keyValidator) : base(logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ClearAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento completo della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

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
                    Status = _cache.TryGetValue(key, out _)
                        ? CacheOperationStatus.Success
                        : CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Errore durante il recupero chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

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

                var entryOptions = new MemoryCacheEntryOptions();

                if (options?.AbsoluteExpiration != null)
                    entryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;

                if (options?.SlidingExpiration != null)
                    entryOptions.SlidingExpiration = options.SlidingExpiration;

                _cache.Set(key, value, entryOptions);
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Errore durante il salvataggio chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }
    }
}