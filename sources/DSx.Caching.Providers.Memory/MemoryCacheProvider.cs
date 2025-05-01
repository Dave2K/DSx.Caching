using Microsoft.Extensions.Caching.Memory;
using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DSx.Caching.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Fornitore di cache in memoria che implementa l'interfaccia <see cref="ICacheProvider"/>.
    /// Gestisce la memorizzazione dei dati e le operazioni thread-safe con lock dedicati.
    /// </summary>
    public sealed class MemoryCacheProvider : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, CacheEntryMetadata> _metadata = new();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();
        private readonly ILogger<MemoryCacheProvider> _logger;
        private bool _disposed;

        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="MemoryCacheProvider"/>.
        /// </summary>
        /// <param name="cache">Istanza di IMemoryCache.</param>
        /// <param name="logger">Logger per tracciare le operazioni.</param>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Recupera un elemento dalla cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento.</typeparam>
        /// <param name="key">Chiave di identificazione.</param>
        /// <param name="options">Opzioni di cache (opzionali).</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Risultato dell'operazione con il valore trovato.</returns>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get));

            try
            {
                var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(cancellationToken);

                try
                {
                    if (_cache.TryGetValue(key, out T? value) && value != null)
                    {
                        if (options?.SlidingExpiration != null)
                        {
                            _cache.Set(key, value, new MemoryCacheEntryOptions
                            {
                                SlidingExpiration = options.SlidingExpiration
                            });
                        }

                        UpdateLastAccess(key);
                        AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, true));
                        return new CacheOperationResult<T>(value, CacheOperationStatus.Success);
                    }

                    _logger.LogWarning("Chiave {Key} non trovata nella cache", key);
                    return new CacheOperationResult<T>(
                        default!,
                        CacheOperationStatus.NotFound,
                        "Chiave non trovata");
                }
                finally
                {
                    keyLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della chiave {Key}", key);
                return new CacheOperationResult<T>(
                    default!,
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Inserisce o aggiorna un elemento nella cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento.</typeparam>
        /// <param name="key">Chiave di identificazione.</param>
        /// <param name="value">Valore da memorizzare.</param>
        /// <param name="options">Opzioni di cache (opzionali).</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set));

            try
            {
                var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(cancellationToken);

                try
                {
                    var entryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = options?.AbsoluteExpiration,
                        SlidingExpiration = options?.SlidingExpiration
                    };

                    _cache.Set(key, value, entryOptions);

                    _metadata[key] = new CacheEntryMetadata(
                        DateTime.UtcNow,
                        options?.AbsoluteExpiration,
                        options?.SlidingExpiration);

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set, true));
                    return new CacheOperationResult(CacheOperationStatus.Success);
                }
                finally
                {
                    keyLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio della chiave {Key}", key);
                return new CacheOperationResult(
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Rimuove un elemento dalla cache.
        /// </summary>
        /// <param name="key">Chiave di identificazione.</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove));

            try
            {
                var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(cancellationToken);

                try
                {
                    _cache.Remove(key);
                    _metadata.TryRemove(key, out _);
                    _keyLocks.TryRemove(key, out _);

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove, true));
                    return new CacheOperationResult(CacheOperationStatus.Success);
                }
                finally
                {
                    keyLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione della chiave {Key}", key);
                return new CacheOperationResult(
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Recupera i metadati di una voce della cache.
        /// </summary>
        /// <param name="key">Chiave di identificazione.</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Descrittore della voce di cache.</returns>
        public async Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));

            try
            {
                var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(cancellationToken);

                try
                {
                    if (!_metadata.TryGetValue(key, out var metadata))
                    {
                        return new CacheOperationResult<CacheEntryDescriptor>(
                            default!,
                            CacheOperationStatus.NotFound,
                            "Chiave non trovata");
                    }

                    var descriptor = new CacheEntryDescriptor(
                        key,
                        metadata.CreatedAt,
                        metadata.LastAccess,
                        metadata.AbsoluteExpiration,
                        metadata.SlidingExpiration,
                        0);

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata, true));
                    return new CacheOperationResult<CacheEntryDescriptor>(descriptor, CacheOperationStatus.Success);
                }
                finally
                {
                    keyLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei metadati per {Key}", key);
                return new CacheOperationResult<CacheEntryDescriptor>(
                    default!,
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache.
        /// </summary>
        /// <param name="key">Chiave da verificare.</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>True se la chiave esiste, altrimenti false.</returns>
        public async Task<CacheOperationResult<bool>> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists));

            try
            {
                var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(cancellationToken);

                try
                {
                    var exists = _cache.TryGetValue(key, out _);
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists, exists));
                    return new CacheOperationResult<bool>(exists, CacheOperationStatus.Success);
                }
                finally
                {
                    keyLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica della chiave {Key}", key);
                return new CacheOperationResult<bool>(
                    false,
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Svuota completamente la cache.
        /// </summary>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            BeforeOperation?.Invoke(this, new CacheEventArgs("ALL", CacheOperationType.ClearAll));

            try
            {
                foreach (var key in _keyLocks.Keys)
                {
                    await RemoveAsync(key, cancellationToken);
                }

                AfterOperation?.Invoke(this, new CacheEventArgs("ALL", CacheOperationType.ClearAll, true));
                return new CacheOperationResult(CacheOperationStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante lo svuotamento della cache");
                return new CacheOperationResult(
                    CacheOperationStatus.ConnectionError,
                    ex.Message);
            }
        }

        /// <summary>
        /// Rilascia le risorse non gestite.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _cache.Dispose();
            foreach (var lockObj in _keyLocks.Values)
            {
                lockObj.Dispose();
            }
            _keyLocks.Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rilascia le risorse non gestite in modo asincrono.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_cache is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _cache.Dispose();
            }

            foreach (var lockObj in _keyLocks.Values)
            {
                lockObj.Dispose();
            }
            _keyLocks.Clear();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #region Metodi Privati
        private void UpdateLastAccess(string key)
        {
            if (_metadata.TryGetValue(key, out var metadata))
            {
                _metadata[key] = metadata.WithLastAccess(DateTime.UtcNow);
            }
        }
        #endregion

        #region Record Interni
        private record CacheEntryMetadata(
            DateTime CreatedAt,
            TimeSpan? AbsoluteExpiration,
            TimeSpan? SlidingExpiration)
        {
            public DateTime LastAccess { get; private init; } = CreatedAt;
            public CacheEntryMetadata WithLastAccess(DateTime lastAccess) => this with { LastAccess = lastAccess };
        }
        #endregion
    }
}
