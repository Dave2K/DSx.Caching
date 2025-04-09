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
    /// Fornisce un'implementazione in-memory per il caching dei dati
    /// </summary>
    public sealed class MemoryCacheProvider(
    IMemoryCache cache,
    ILogger<MemoryCacheProvider> logger,
    ICacheKeyValidator keyValidator)
    : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ILogger<MemoryCacheProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ICacheKeyValidator _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));

        private bool _disposed = false;

        /// <inheritdoc/>
        public Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var exists = _cache.TryGetValue(key, out _);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Verifica esistenza annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore verifica esistenza chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public Task<CacheOperationResult<T>> GetAsync<T>(
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
                    if (value is null) // Controllo esplicito per null
                    {
                        return Task.FromResult(new CacheOperationResult<T>
                        {
                            Status = CacheOperationStatus.NotFound
                        });
                    }

                    return Task.FromResult(new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = value // Non serve ! grazie al controllo precedente
                    });
                }

                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.NotFound
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Lettura annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore lettura chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public Task<CacheOperationResult> SetAsync<T>(
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
                _logger.LogWarning("Scrittura annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore scrittura chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public Task<CacheOperationResult> RemoveAsync(
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
                _logger.LogWarning("Rimozione annullata per chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore rimozione chiave: {Key}", key);
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_cache is MemoryCache memoryCache)
                    memoryCache.Compact(1.0); // Compatta il 100% della cache

                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Pulizia cache annullata");
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore pulizia globale cache");
                return Task.FromResult(new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                });
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_disposed)
            {
                _cache?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}