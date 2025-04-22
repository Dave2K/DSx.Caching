using DSx.Caching.Abstractions.Events;
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
    /// Fornitore di cache in memoria che implementa le funzionalità di caching
    /// </summary>
    public sealed class MemoryCacheProvider : BaseCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly MemoryCacheEntryOptions _defaultOptions;

        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache
        /// </summary>
        public override event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache
        /// </summary>
        public override event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Evento sollevato quando un'operazione viene differita
        /// </summary>
        public override event EventHandler<OperationDeferredEventArgs>? OperationDeferred;

        /// <summary>
        /// Inizializza una nuova istanza del provider di cache in memoria
        /// </summary>
        /// <param name="cache">Istanza della cache in memoria</param>
        /// <param name="logger">Logger per la tracciatura delle attività</param>
        /// <param name="keyValidator">Validatore per le chiavi della cache</param>
        /// <param name="defaultExpiration">Scadenza predefinita per le voci della cache</param>
        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            TimeSpan? defaultExpiration = null) : base(logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));

            _defaultOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(defaultExpiration ?? TimeSpan.FromMinutes(30));

            _defaultOptions.RegisterPostEvictionCallback(EvictionCallback);
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato dell'operazione con stato di esistenza</returns>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var exists = _cache.TryGetValue(key, out _);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists, exists));

                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica della chiave {Key}", key);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists, false));
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave associata al valore</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato dell'operazione con il valore recuperato</returns>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                if (!_cache.TryGetValue(key, out CacheEntryWrapper? wrapper) || wrapper == null)
                {
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, false));
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };
                }

                if (wrapper.Value is T typedValue)
                {
                    wrapper.UpdateOnRead();
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, true));
                    return new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = typedValue
                    };
                }

                Logger.LogError("Tipo non compatibile per la chiave {Key}", key);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, false));
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.SerializationError,
                    Details = $"Tipo atteso: {typeof(T).Name}, Tipo effettivo: {wrapper.Value.GetType().Name}"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero della chiave {Key}", key);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, false));
                return new CacheOperationResult<T> { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave associata al valore</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni per la memorizzazione</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set));

            try
            {
                // Verifica prima la cancellazione
                cancellationToken.ThrowIfCancellationRequested(); // <-- Spostato qui
                _keyValidator.Validate(key); // <-- Dopo il controllo di cancellazione

                var entryOptions = options != null ?
                    CreateMemoryCacheEntryOptions(options) :
                    _defaultOptions;

                var wrapper = new CacheEntryWrapper(value!, CalculateSize(value));
                _cache.Set(key, wrapper, entryOptions);

                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set, true));
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Operazione annullata per la chiave {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio della chiave {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Rimuove una voce dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                _cache.Remove(key);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove, true));
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione della chiave {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs("ALL_KEYS", CacheOperationType.ClearAll));

            try
            {
                if (_cache is MemoryCache memoryCache)
                {
                    memoryCache.Compact(1.0);
                    AfterOperation?.Invoke(this, new CacheEventArgs("ALL_KEYS", CacheOperationType.ClearAll, true));
                    return new CacheOperationResult { Status = CacheOperationStatus.Success };
                }

                return new CacheOperationResult { Status = CacheOperationStatus.ValidationError };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento della cache");
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Ottiene i metadati di una voce della cache
        /// </summary>
        /// <param name="key">Chiave da interrogare</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Descrittore della voce o null se non presente</returns>
        public override async Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                if (!_cache.TryGetValue(key, out CacheEntryWrapper? wrapper) || wrapper == null)
                    return null;

                return new CacheEntryDescriptor(
                    wrapper.LastAccessed,
                    wrapper.ReadCount,
                    wrapper.SizeInBytes,
                    wrapper.IsDirty
                );
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero dei metadati per {Key}", key);
                return null;
            }
            finally
            {
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));
            }
        }

        private static long CalculateSize<T>(T value)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value).Length;
        }

        private MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(CacheEntryOptions? options)
        {
            var entryOptions = new MemoryCacheEntryOptions();

            if (options?.AbsoluteExpiration != null)
                entryOptions.SetAbsoluteExpiration(options.AbsoluteExpiration.Value);

            if (options?.SlidingExpiration != null)
                entryOptions.SetSlidingExpiration(options.SlidingExpiration.Value);

            entryOptions.RegisterPostEvictionCallback(EvictionCallback);

            return entryOptions;
        }

        private void EvictionCallback(object key, object? value, EvictionReason reason, object? state)
        {
            if (value is CacheEntryWrapper wrapper)
            {
                Logger.LogInformation(
                    "Elemento rimosso: {Key}. Motivo: {Reason} - Ultimo accesso: {LastAccess}",
                    key, reason, wrapper.LastAccessed
                );

                OperationDeferred?.Invoke(this, new OperationDeferredEventArgs(
                    key.ToString() ?? string.Empty,
                    reason.ToString()
                ));
            }
        }

        private class CacheEntryWrapper(object value, long initialSize)
        {
            public DateTime LastAccessed { get; private set; } = DateTime.UtcNow;
            public int ReadCount { get; private set; }
            public long SizeInBytes { get; private set; } = initialSize;
            public bool IsDirty { get; private set; } = true;
            public object Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

            public void UpdateOnRead()
            {
                ReadCount++;
                LastAccessed = DateTime.UtcNow;
            }

            public void UpdateOnWrite(long newSize)
            {
                SizeInBytes = newSize;
                IsDirty = true;
                LastAccessed = DateTime.UtcNow;
            }

            public void MarkAsClean()
            {
                IsDirty = false;
            }
        }

        /// <summary>
        /// Rilascia le risorse non gestite utilizzate da MemoryCacheProvider 
        /// e opzionalmente rilascia le risorse gestite.
        /// </summary>
        /// <param name="disposing">
        /// true per rilasciare sia le risorse gestite che non gestite; 
        /// false per rilasciare solo le risorse non gestite.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cache.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}