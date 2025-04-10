using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core;
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
    /// Implementazione di un provider di cache utilizzando Redis come backend
    /// </summary>
    public sealed class RedisCacheProvider(
        IConnectionMultiplexer connection,
        ILogger<RedisCacheProvider> logger,
        ICacheKeyValidator keyValidator,
        IOptions<JsonSerializerOptions> jsonOptions)
        : BaseCacheProvider(logger)
    {
        private readonly IConnectionMultiplexer _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        private readonly IDatabase _database = connection.GetDatabase();
        private readonly ICacheKeyValidator _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
        private readonly JsonSerializerOptions _serializerOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var exists = await _database.KeyExistsAsync(key).ConfigureAwait(false);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
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
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var redisValue = await _database.StringGetAsync(key).ConfigureAwait(false);

                if (redisValue.IsNullOrEmpty)
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };

                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions)
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = ex is RedisException ?
                        CacheOperationStatus.ConnectionError :
                        CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
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

                await _database.StringSetAsync(key, serializedValue, expiry).ConfigureAwait(false);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = ex is RedisException ?
                        CacheOperationStatus.ConnectionError :
                        CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var deleted = await _database.KeyDeleteAsync(key).ConfigureAwait(false);
                return new CacheOperationResult
                {
                    Status = deleted ?
                        CacheOperationStatus.Success :
                        CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var endpoint in _connection.GetEndPoints())
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushAllDatabasesAsync().ConfigureAwait(false);
                }

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento completo della cache");
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
                Dispose(false);
                GC.SuppressFinalize(this);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                }
                base.Dispose(disposing);
                _disposed = true;
            }
        }
    }
}