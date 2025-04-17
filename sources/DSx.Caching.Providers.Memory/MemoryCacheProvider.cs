using DSx.Caching.Abstractions.Interfaces;
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
    /// Provider di cache in memoria che implementa l'interfaccia <see cref="ICacheProvider"/>
    /// </summary>
    public sealed class MemoryCacheProvider(
        IMemoryCache cache,
        ILogger<MemoryCacheProvider> logger,
        ICacheKeyValidator keyValidator) : BaseCacheProvider(logger)
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ICacheKeyValidator _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ClearAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_cache is MemoryCache memoryCache) memoryCache.Compact(1.0);
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (OperationCanceledException)
            {
                return new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento completo della cache");
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni della cache</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato con stato dell'operazione</returns>
        public override async Task<CacheOperationResult> ExistsAsync(
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
                    Status = _cache.TryGetValue(key, out _)
                        ? CacheOperationStatus.Success
                        : CacheOperationStatus.NotFound
                });
            }
            catch (OperationCanceledException)
            {
                return new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ValidationError };
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni della cache</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato con valore e stato</returns>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
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
            catch (OperationCanceledException)
            {
                return new CacheOperationResult<T> { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero chiave: {Key}", key);
                return new CacheOperationResult<T> { Status = CacheOperationStatus.ValidationError };
            }
        }

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);
                _cache.Remove(key);
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (OperationCanceledException)
            {
                return new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ValidationError };
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave di memorizzazione</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni della cache</param>
        /// <param name="cancellationToken">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                var entryOptions = new MemoryCacheEntryOptions();
                if (options?.AbsoluteExpiration != null)
                    entryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;

                if (options?.SlidingExpiration != null)
                    entryOptions.SlidingExpiration = options.SlidingExpiration;

                _cache.Set(key, value, entryOptions);
                return await Task.FromResult(new CacheOperationResult { Status = CacheOperationStatus.Success });
            }
            catch (OperationCanceledException)
            {
                return new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ValidationError };
            }
        }
    }
}