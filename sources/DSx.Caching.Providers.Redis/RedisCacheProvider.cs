using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Caching;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Fornitore di cache distribuita basato su Redis con gestione avanzata delle operazioni
    /// </summary>
    public sealed class RedisCacheProvider : BaseCacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private EventHandler<OperationDeferredEventArgs>? _operationDeferredHandlers;
        private readonly object _eventLock = new();

        /// <summary>
        /// Evento sollevato prima dell'esecuzione di un'operazione sulla cache
        /// </summary>
        public override event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo il completamento di un'operazione sulla cache
        /// </summary>
        public override event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Evento sollevato quando un'operazione viene posticipata
        /// </summary>
        public override event EventHandler<OperationDeferredEventArgs>? OperationDeferred
        {
            add
            {
                lock (_eventLock)
                {
                    _operationDeferredHandlers += value;
                }
            }
            remove
            {
                lock (_eventLock)
                {
                    _operationDeferredHandlers -= value;
                }
            }
        }

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        /// <param name="connection">Connessione multiplexer a Redis</param>
        /// <param name="logger">Istanza del logger per la tracciatura</param>
        /// <param name="keyValidator">Validatore per le chiavi di cache</param>
        /// <param name="jsonOptions">Opzioni di configurazione per la serializzazione JSON</param>
        /// <exception cref="ArgumentNullException">Generata quando i parametri obbligatori sono null</exception>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions) : base(logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _database = _connection.GetDatabase();
            _serializerOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            _operationDeferredHandlers = null;
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Identificativo della chiave da verificare</param>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Risultato dell'operazione con lo stato di esistenza</returns>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var exists = await _database.KeyExistsAsync(key).ConfigureAwait(false);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists, exists));

                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante verifica esistenza chiave {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del dato da recuperare</typeparam>
        /// <param name="key">Identificativo della chiave</param>
        /// <param name="options">Opzioni aggiuntive per il recupero</param>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Risultato contenente il valore recuperato</returns>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var redisValue = await _database.StringGetAsync(key).ConfigureAwait(false);

                if (redisValue.IsNullOrEmpty)
                {
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, false));
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };
                }

                var value = JsonSerializer.Deserialize<T>(redisValue!, _serializerOptions);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, true));
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = value
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante recupero valore chiave {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ConnectionError
                };
            }
        }

        /// <summary>
        /// Ottiene i metadati di una voce della cache
        /// </summary>
        /// <param name="key">Identificativo della voce</param>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Descrittore della voce o null se non trovata</returns>
        public override async Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var ttl = await _database.KeyTimeToLiveAsync(key).ConfigureAwait(false);
                if (!ttl.HasValue) return null;

                return new CacheEntryDescriptor(
                    key: key,
                    lastAccessed: DateTime.UtcNow - ttl.Value,
                    readCount: 0,
                    sizeInBytes: 0,
                    isDirty: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore recupero metadati chiave {Key}", key);
                return null;
            }
            finally
            {
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del dato da memorizzare</typeparam>
        /// <param name="key">Identificativo della chiave</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di memorizzazione</param>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set));

            try
            {
                _keyValidator.Validate(key); // Validazione esplicita
                cancellationToken.ThrowIfCancellationRequested();

                var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration;

                await _database.StringSetAsync(key, serializedValue, expiry).ConfigureAwait(false);

                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set, true));
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (ArgumentException ex) // Cattura eccezione di validazione
            {
                Logger.LogError(ex, "Chiave non valida: {Key}", key);
                throw; // Rilancia per il test
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
        /// <param name="key">Identificativo della chiave</param>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove));

            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                await _database.KeyDeleteAsync(key).ConfigureAwait(false);
                AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove, true));
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante rimozione chiave {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Svuota completamente il contenuto della cache
        /// </summary>
        /// <param name="cancellationToken">Token per l'annullamento asincrono</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            BeforeOperation?.Invoke(this, new CacheEventArgs("ALL_KEYS", CacheOperationType.ClearAll));

            try
            {
                var endpoints = _connection.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushDatabaseAsync().ConfigureAwait(false);
                }
                AfterOperation?.Invoke(this, new CacheEventArgs("ALL_KEYS", CacheOperationType.ClearAll, true));
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante pulizia completa cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rilascia le risorse gestite della connessione Redis
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            await _connection.CloseAsync().ConfigureAwait(false);
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }
}
