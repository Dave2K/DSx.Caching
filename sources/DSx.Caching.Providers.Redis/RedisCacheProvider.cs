using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
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
    /// Provider di cache basato su Redis
    /// </summary>
    public class RedisCacheProvider(
        IConnectionMultiplexer connection,
        ILogger<RedisCacheProvider> logger,
        ICacheKeyValidator keyValidator,
        IOptions<JsonSerializerOptions> jsonOptions) : BaseCacheProvider(logger)
    {
        private readonly IConnectionMultiplexer _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        private readonly IDatabase _database = connection.GetDatabase();
        private readonly ICacheKeyValidator _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        private readonly JsonSerializerOptions _serializerOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
        private bool _disposed;

        /// <summary>
        /// Verifica se una chiave esiste nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni della cache (opzionale)</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var exists = await _database.KeyExistsAsync(key);
                return new CacheOperationResult
                {
                    Status = exists
                        ? CacheOperationStatus.Success
                        : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                return HandleError<object>(ex, nameof(ExistsAsync), key);
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
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                // Forza la connessione a Redis prima di eseguire l'operazione
                if (!_connection.IsConnected)
                {
                    throw new RedisConnectionException(
                        ConnectionFailureType.UnableToResolvePhysicalConnection,
                        "Connessione Redis non disponibile");
                }

                var redisValue = await _database.StringGetAsync(key);

                if (redisValue.IsNullOrEmpty)
                {
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };
                }

                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions)
                };
            }
            catch (RedisConnectionException)
            {
                // Rilancia senza log per il test
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore generico per chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
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

                var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration ?? options?.SlidingExpiration;

                await _database.StringSetAsync(key, serializedValue, expiry);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore scrittura chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rimuove un valore dalla cache
        /// </summary>
        /// <param name="key">Chiave del valore da rimuovere</param>
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var deleted = await _database.KeyDeleteAsync(key);
                return new CacheOperationResult
                {
                    Status = deleted ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore rimozione chiave: {Key}", key);
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
        /// <param name="cancellationToken">Token di cancellazione (opzionale)</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var endpoint in _connection.GetEndPoints())
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushAllDatabasesAsync();
                }

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore pulizia globale cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rilascia le risorse
        /// </summary>
        public override void Dispose()
        {
            if (!_disposed)
            {
                _connection?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Rilascia le risorse in modo asincrono
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_connection != null)
                {
                    await _connection.CloseAsync().ConfigureAwait(false);
                    await _connection.DisposeAsync().ConfigureAwait(false);
                }
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}