using DSx.Caching.Abstractions.Configurations;
using DSx.Caching.SharedKernel.Exceptions;
using DSx.Caching.Abstractions.Factories;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Core;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Provider di cache basato su Redis
    /// </summary>
    public class RedisCacheProvider : BaseCacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly RedisCacheProviderConfiguration _config;

        /// <summary>
        /// Costruttore principale
        /// </summary>
        public RedisCacheProvider(
            IConnectionMultiplexerFactory connectionFactory,
            IOptions<RedisCacheProviderConfiguration> config,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions)
            : base(logger)
        {
            _config = config.Value;
            _connection = connectionFactory.CreateConnection(_config.ConnectionString);
            _database = _connection.GetDatabase();
            _keyValidator = keyValidator;
            _serializerOptions = jsonOptions.Value;
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var exists = await _database.KeyExistsAsync(key);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (RedisException ex)
            {
                Logger.LogError(ex, "Errore Redis per la chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var redisValue = await _database.StringGetAsync(key);

                if (redisValue.IsNullOrEmpty)
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };

                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions)
                };
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "Errore deserializzazione per {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
            catch (RedisException ex)
            {
                Logger.LogError(ex, "Errore connessione Redis per {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration ?? options?.SlidingExpiration;

                await _database.StringSetAsync(
                    key,
                    serializedValue,
                    expiry,
                    When.Always);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (RedisException ex)
            {
                Logger.LogError(ex, "Errore salvataggio chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var deleted = await _database.KeyDeleteAsync(key);
                return new CacheOperationResult
                {
                    Status = deleted ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (RedisException ex)
            {
                Logger.LogError(ex, "Errore rimozione chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var endpoints = _connection.GetEndPoints();

                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushAllDatabasesAsync();
                }

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (RedisException ex)
            {
                Logger.LogError(ex, "Errore svuotamento cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await base.DisposeAsync().ConfigureAwait(false);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _connection?.Dispose();
                base.Dispose(disposing);
                _disposed = true;
            }
        }
    }
}