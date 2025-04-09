using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Implementazione Redis del provider di cache
    /// </summary>
    public sealed class RedisCacheProvider : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheProvider> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        public RedisCacheProvider(
            string connectionString,
            ILogger<RedisCacheProvider> logger,
            IConnectionMultiplexer? connection = null,
            JsonSerializerOptions? serializerOptions = null)
        {
            _connection = connection ?? ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
            _logger = logger;
            _serializerOptions = serializerOptions ?? new JsonSerializerOptions();
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
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                var exists = await _database.KeyExistsAsync(key);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore Redis [Exists] per chiave: {Key}", key);
                throw new RedisCacheException($"Verifica chiave '{key}' fallita", ex);
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                var redisValue = await _database.StringGetAsync(key);
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
                _logger.LogError(ex, "Errore Redis [Get] per chiave: {Key}", key);
                throw new RedisCacheException($"Recupero chiave '{key}' fallito", ex);
            }
        }

        /// <summary>
        /// Imposta un valore nella cache
        /// </summary>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                await _database.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(value, _serializerOptions),
                    options?.AbsoluteExpiration ?? options?.SlidingExpiration);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore Redis [Set] per chiave: {Key}", key);
                throw new RedisCacheException($"Salvataggio chiave '{key}' fallito", ex);
            }
        }

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                CacheKeyValidator.ThrowIfInvalid(key);

                await _database.KeyDeleteAsync(key);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore Redis [Remove] per chiave: {Key}", key);
                throw new RedisCacheException($"Rimozione chiave '{key}' fallita", ex);
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
                    await _connection.GetServer(endpoint).FlushAllDatabasesAsync();
                }
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore Redis [ClearAll]");
                throw new RedisCacheException("Pulizia cache fallita", ex);
            }
        }

        /// <summary>
        /// Dispose sincrono
        /// </summary>
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose asincrono
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}