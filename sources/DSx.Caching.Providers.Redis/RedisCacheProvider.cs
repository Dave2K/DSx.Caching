using StackExchange.Redis;
using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using DSx.Caching.SharedKernel.Interfaces;
using DSx.Caching.SharedKernel.Validation;
using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Interfaces;
using System.Text.Json;
using System.Globalization;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Fornisce un'implementazione di <see cref="ICacheProvider"/> per Redis con resilienza integrata e gestione degli errori.
    /// </summary>
    public sealed class RedisCacheProvider : ICacheProvider, IAsyncDisposable, IDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<RedisCacheProvider> _logger;
        private readonly ICacheCircuitBreaker _circuitBreaker;
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
        /// Inizializza una nuova istanza della classe <see cref="RedisCacheProvider"/>.
        /// </summary>
        /// <param name="connection">Connessione a Redis.</param>
        /// <param name="logger">Logger per la tracciatura delle attività.</param>
        /// <param name="keyValidator">Validatore per le chiavi della cache.</param>
        /// <param name="jsonOptions">Opzioni per la serializzazione JSON.</param>
        /// <param name="circuitBreaker">Circuit breaker per la gestione degli errori.</param>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            JsonSerializerOptions jsonOptions,
            ICacheCircuitBreaker circuitBreaker)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _serializerOptions = jsonOptions ?? throw new ArgumentNullException(nameof(jsonOptions));
            _circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
            _database = _connection.GetDatabase();
        }

        /// <summary>
        /// Recupera un elemento dalla cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento.</typeparam>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="options">Opzioni della cache (ignorate in Redis).</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione con l'elemento recuperato.</returns>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "GET",
                key,
                async (ct) =>
                {
                    _keyValidator.Validate(key);
                    BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get));

                    var redisValue = await _database.StringGetAsync(key).ConfigureAwait(false);
                    if (redisValue.IsNullOrEmpty)
                    {
                        AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, false));
                        return new CacheOperationResult<T>(
                            default!,
                            CacheOperationStatus.NotFound,
                            $"Elemento non trovato: {key}");
                    }

                    var value = JsonSerializer.Deserialize<T>(redisValue!, _serializerOptions);
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Get, true));
                    return new CacheOperationResult<T>(value!, CacheOperationStatus.Success);
                },
                fallbackValue: new CacheOperationResult<T>(
                    default!,
                    CacheOperationStatus.ConnectionError,
                    "Fallback: Servizio non disponibile"),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Memorizza un elemento nella cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento.</typeparam>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="value">Valore da memorizzare.</param>
        /// <param name="options">Opzioni di scadenza.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "SET",
                key,
                async (ct) =>
                {
                    _keyValidator.Validate(key);
                    var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
                    var expiry = options?.AbsoluteExpiration;

                    BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set));
                    var success = await _database.StringSetAsync(key, serializedValue, expiry).ConfigureAwait(false);

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Set, success));
                    return success
                        ? new CacheOperationResult(CacheOperationStatus.Success)
                        : new CacheOperationResult(CacheOperationStatus.ConnectionError);
                },
                fallbackValue: new CacheOperationResult(CacheOperationStatus.ConnectionError),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Rimuove un elemento dalla cache.
        /// </summary>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "REMOVE",
                key,
                async (ct) =>
                {
                    _keyValidator.Validate(key);
                    BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove));
                    var result = await _database.KeyDeleteAsync(key).ConfigureAwait(false);

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Remove, result));
                    return result
                        ? new CacheOperationResult(CacheOperationStatus.Success)
                        : new CacheOperationResult(CacheOperationStatus.NotFound);
                },
                fallbackValue: new CacheOperationResult(CacheOperationStatus.ConnectionError),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Recupera i metadati di una voce della cache.
        /// </summary>
        /// <param name="key">Chiave della voce.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Metadati della voce o errore.</returns>
        public async Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "GET_DESCRIPTOR",
                key,
                async (ct) =>
                {
                    _keyValidator.Validate(key);
                    BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata));

                    var exists = await _database.KeyExistsAsync(key).ConfigureAwait(false);
                    if (!exists)
                    {
                        return new CacheOperationResult<CacheEntryDescriptor>(
                            default!,
                            CacheOperationStatus.NotFound,
                            $"Chiave {key} non trovata");
                    }

                    var ttl = await _database.KeyTimeToLiveAsync(key).ConfigureAwait(false);
                    var valueLength = await _database.StringLengthAsync(key).ConfigureAwait(false);

                    var descriptor = new CacheEntryDescriptor(
                        key,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        ttl,
                        null,
                        valueLength * sizeof(char)
                    );

                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.GetMetadata, true));
                    return new CacheOperationResult<CacheEntryDescriptor>(descriptor, CacheOperationStatus.Success);
                },
                fallbackValue: new CacheOperationResult<CacheEntryDescriptor>(
                    default!,
                    CacheOperationStatus.ConnectionError),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache.
        /// </summary>
        /// <param name="key">Chiave da verificare.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato della verifica.</returns>
        public async Task<CacheOperationResult<bool>> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "EXISTS",
                key,
                async (ct) =>
                {
                    _keyValidator.Validate(key);
                    BeforeOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists));

                    var exists = await _database.KeyExistsAsync(key).ConfigureAwait(false);
                    AfterOperation?.Invoke(this, new CacheEventArgs(key, CacheOperationType.Exists, exists));

                    return new CacheOperationResult<bool>(exists, CacheOperationStatus.Success);
                },
                fallbackValue: new CacheOperationResult<bool>(false, CacheOperationStatus.ConnectionError),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Svuota completamente la cache.
        /// </summary>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        /// <returns>Risultato dell'operazione.</returns>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithResilienceAsync(
                operationName: "CLEAR_ALL",
                "ALL_KEYS",
                async (ct) =>
                {
                    BeforeOperation?.Invoke(this, new CacheEventArgs("ALL", CacheOperationType.ClearAll));
                    var endpoints = _connection.GetEndPoints(configuredOnly: true);

                    foreach (var endpoint in endpoints)
                    {
                        var server = _connection.GetServer(endpoint);
                        await server.FlushDatabaseAsync().ConfigureAwait(false);
                    }

                    AfterOperation?.Invoke(this, new CacheEventArgs("ALL", CacheOperationType.ClearAll, true));
                    return new CacheOperationResult(CacheOperationStatus.Success);
                },
                fallbackValue: new CacheOperationResult(CacheOperationStatus.ConnectionError),
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Libera le risorse non gestite.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _connection.Dispose();
            (_circuitBreaker as IDisposable)?.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Libera le risorse non gestite in modo asincrono.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            await _connection.DisposeAsync().ConfigureAwait(false);
            if (_circuitBreaker is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            _disposed = true;
        }

        #region Metodi Privati

        private async Task<T> ExecuteWithResilienceAsync<T>(
            string operationName,
            string key,
            Func<CancellationToken, Task<T>> operation,
            T fallbackValue,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _circuitBreaker.ExecuteAsync(
                    async (ct) =>
                    {
                        using (ct.Register(() =>
                            _logger.LogWarning("Operazione {Operation} annullata per {Key}", operationName, key)))
                        {
                            _logger.LogDebug("Inizio operazione {Operation} su {Key}", operationName, key);
                            var result = await operation(ct).ConfigureAwait(false);
                            _logger.LogDebug("Operazione {Operation} completata per {Key}", operationName, key);
                            return result;
                        }
                    },
                    (ct) => Task.FromResult(fallbackValue),
                    cancellationToken
                ).ConfigureAwait(false);
            }
            catch (InvalidCacheKeyException ex)
            {
                _logger.LogWarning(ex, "Validazione chiave fallita per {Key}: {ValidationRules}", key, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore non gestito durante {Operation} su {Key}: {ErrorMessage}",
                    operationName,
                    key,
                    ex.Message);
                throw;
            }
        }

        #endregion
    }
}
