using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Implementazione di un provider di cache basato su Redis.
    /// </summary>
    public class RedisCacheProvider : BaseCacheProvider, ICacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="RedisCacheProvider"/>.
        /// </summary>
        /// <param name="connection">Connessione a Redis.</param>
        /// <param name="logger">Logger per la registrazione degli eventi.</param>
        /// <param name="keyValidator">Validatore per le chiavi della cache.</param>
        /// <param name="jsonOptions">Opzioni per la serializzazione JSON.</param>
        /// <exception cref="ArgumentNullException">Generata quando un parametro obbligatorio è null.</exception>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions)
            : base(logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _database = connection.GetDatabase();
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _serializerOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
        }

        /// <summary>
        /// Verifica se una chiave esiste nella cache.
        /// </summary>
        /// <param name="key">Chiave da verificare.</param>
        /// <param name="options">Opzioni della cache (opzionale).</param>
        /// <param name="cancellationToken">Token di annullamento (opzionale).</param>
        /// <returns>Risultato dell'operazione.</returns>
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
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                return HandleError<object>(ex, nameof(ExistsAsync), key);
            }
        }

        /// <summary>
        /// Recupera un elemento dalla cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento da recuperare.</typeparam>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="options">Opzioni della cache (opzionale).</param>
        /// <param name="cancellationToken">Token di annullamento (opzionale).</param>
        /// <returns>Risultato contenente l'elemento recuperato.</returns>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
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
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };

                if (typeof(T) == typeof(string))
                {
                    var stringValue = redisValue.ToString();
                    if (stringValue.StartsWith('"') && stringValue.EndsWith('"'))
                    {
                        stringValue = stringValue[1..^1];
                    }

                    return new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = (T)(object)stringValue
                    };
                }

                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions)
                };
            }
            catch (Exception ex) when (ex is JsonException or ArgumentException)
            {
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
            catch (Exception ex) when (ex is RedisException or TimeoutException)
            {
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Memorizza un elemento nella cache.
        /// </summary>
        /// <typeparam name="T">Tipo dell'elemento da memorizzare.</typeparam>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="value">Valore da memorizzare.</param>
        /// <param name="options">Opzioni della cache (opzionale).</param>
        /// <param name="cancellationToken">Token di annullamento (opzionale).</param>
        /// <returns>Risultato dell'operazione.</returns>
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

                await _database.StringSetAsync(key, serializedValue, expiry, When.Always, CommandFlags.None);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                return HandleError<object>(ex, nameof(SetAsync), key);
            }
        }

        /// <summary>
        /// Rimuove un elemento dalla cache.
        /// </summary>
        /// <param name="key">Chiave dell'elemento da rimuovere.</param>
        /// <param name="cancellationToken">Token di annullamento (opzionale).</param>
        /// <returns>Risultato dell'operazione.</returns>
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
                return HandleError<object>(ex, nameof(RemoveAsync), key);
            }
        }

        /// <summary>
        /// Svuota completamente la cache.
        /// </summary>
        /// <param name="cancellationToken">Token di annullamento (opzionale).</param>
        /// <returns>Risultato dell'operazione.</returns>
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
                return HandleError<object>(ex, nameof(ClearAllAsync), "ALL_KEYS");
            }
        }

        /// <summary>
        /// Rilascia le risorse non gestite.
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
        /// Rilascia le risorse non gestite in modo asincrono.
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