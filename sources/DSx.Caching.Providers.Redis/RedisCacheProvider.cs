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
    /// Provider di caching basato su Redis per operazioni avanzate di memorizzazione e recupero dati
    /// </summary>
    public class RedisCacheProvider : ICacheProvider, IAsyncDisposable, IDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisCacheProvider> _logger;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IDatabase _database;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        /// <param name="connection">Connessione multiplexer a Redis</param>
        /// <param name="logger">Logger per tracciamento attività</param>
        /// <param name="keyValidator">Validatore formale delle chiavi</param>
        /// <param name="jsonOptions">Configurazione serializzazione JSON</param>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _serializerOptions = jsonOptions.Value;
            _database = _connection.GetDatabase();
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        public async Task<CacheOperationResult> ExistsAsync(
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
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore verifica esistenza chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Recupera un elemento dalla cache
        /// </summary>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

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
            catch (RedisConnectionException ex)
            {
                _logger.LogCritical(ex, "Errore di connessione Redis per chiave: {Key}", key);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generico per chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Memorizza un elemento nella cache
        /// </summary>
        public async Task<CacheOperationResult> SetAsync<T>(
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
                _logger.LogError(ex, "Errore scrittura chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        public async Task<CacheOperationResult> RemoveAsync(
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
                _logger.LogError(ex, "Errore rimozione chiave: {Key}", key);
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
        public async Task<CacheOperationResult> ClearAllAsync(
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
                _logger.LogError(ex, "Errore pulizia globale cache");
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rilascia le risorse gestite in modo asincrono
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione del dispose sincrono
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Implementazione del dispose asincrono
        /// </summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await _connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}