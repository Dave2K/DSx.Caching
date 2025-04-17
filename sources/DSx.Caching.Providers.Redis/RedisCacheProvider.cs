using DSx.Caching.Abstractions.Interfaces;
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
    /// Implementazione di un provider di cache basato su Redis
    /// </summary>
    public sealed class RedisCacheProvider : BaseCacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions) : base(logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _database = _connection.GetDatabase();
            _serializerOptions = jsonOptions.Value;
        }

        /// <summary>
        /// Svuota completamente la cache Redis
        /// </summary>
        public override async Task<CacheOperationResult> ClearAllAsync(CancellationToken ct = default)
        {
            try
            {
                CheckDisposed();
                var endpoints = _connection.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushAllDatabasesAsync();
                }
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento della cache Redis");
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                CheckDisposed();
                _keyValidator.Validate(key);
                var exists = await _database.KeyExistsAsync(key);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica della chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Recupera un valore dalla cache Redis
        /// </summary>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                CheckDisposed();
                _keyValidator.Validate(key);
                var value = await _database.StringGetAsync(key);

                if (!value.HasValue)
                    return new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound };

                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = JsonSerializer.Deserialize<T>(value!, _serializerOptions)
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il recupero della chiave: {Key}", key);
                return new CacheOperationResult<T> { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache Redis
        /// </summary>
        public override async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                CheckDisposed();
                _keyValidator.Validate(key);
                var serialized = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration;

                await _database.StringSetAsync(
                    key,
                    serialized,
                    expiry);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio della chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Rimuove una chiave dalla cache Redis
        /// </summary>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken ct = default)
        {
            try
            {
                CheckDisposed();
                _keyValidator.Validate(key);
                await _database.KeyDeleteAsync(key);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione della chiave: {Key}", key);
                return new CacheOperationResult { Status = CacheOperationStatus.ConnectionError };
            }
        }

        /// <summary>
        /// Rilascia le risorse in modo asincrono
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await base.DisposeAsync().ConfigureAwait(false);
                _disposed = true;
            }
        }

        /// <summary>
        /// Rilascia le risorse
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _connection?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}