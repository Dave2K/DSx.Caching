using DSx.Caching.Abstractions;
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
    /// Provider di cache basato su MemoryCache
    /// </summary>
    public class MemoryCacheProvider(
        IMemoryCache cache,
        ILogger<MemoryCacheProvider> logger,
        ICacheKeyValidator keyValidator) : BaseCacheProvider(logger)
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ICacheKeyValidator _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        private bool _disposed;

        /// <summary>
        /// Verifica se una chiave esiste nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni della cache (opzionale)</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
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
                    Status = _cache.TryGetValue(key, out _)
                        ? CacheOperationStatus.Success
                        : CacheOperationStatus.NotFound
                });
            }
            catch (Exception ex)
            {
                var errorResult = HandleError<object>(ex, nameof(ExistsAsync), key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = errorResult.Status,
                    Details = errorResult.Details
                });
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave del valore</param>
        /// <param name="options">Opzioni della cache (opzionale)</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione con il valore</returns>
        public override Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache.TryGetValue(key, out T? value))
                {
                    if (value is null)
                    {
                        return Task.FromResult(new CacheOperationResult<T>
                        {
                            Status = CacheOperationStatus.NotFound
                        });
                    }

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
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Lettura annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore lettura chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave del valore</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni della cache (opzionale)</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
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
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Scrittura annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore scrittura chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Rimuove un valore dalla cache
        /// </summary>
        /// <param name="key">Chiave del valore da rimuovere</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
        public override Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                _cache.Remove(key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Rimozione annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore rimozione chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
        public override Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0);

                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Pulizia cache annullata");
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore pulizia globale cache");
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Rilascia le risorse
        /// </summary>
        public override void Dispose()
        {
            if (!_disposed)
            {
                _cache?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Rilascia le risorse in modo asincrono
        /// </summary>
        public override ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}