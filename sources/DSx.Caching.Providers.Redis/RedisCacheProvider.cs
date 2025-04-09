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
    /// Provider di cache Redis con gestione avanzata delle operazioni
    /// </summary>
    public sealed class RedisCacheProvider : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheProvider> _logger;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        /// <param name="connection">Connessione multiplexer Redis</param>
        /// <param name="logger">Logger per tracciamento attività</param>
        /// <param name="keyValidator">Validatore delle chiavi di cache</param>
        /// <param name="serializerOptions">Opzioni di serializzazione JSON</param>
        /// <exception cref="ArgumentNullException">Se uno dei parametri obbligatori è null</exception>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            JsonSerializerOptions? serializerOptions = null)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _database = _connection.GetDatabase();
            _serializerOptions = serializerOptions ?? new JsonSerializerOptions();
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ClearAllAsync(CancellationToken cancellationToken = default)
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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Errore durante la pulizia completa della cache");
                throw new RedisCacheException("Pulizia cache fallita", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                var exists = await _database.KeyExistsAsync(key);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
                throw new RedisCacheException($"Verifica chiave '{key}' fallita", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                var redisValue = await _database.StringGetAsync(key);

                return redisValue.IsNullOrEmpty
                    ? new CacheOperationResult<T> { Status = CacheOperationStatus.NotFound }
                    : new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.Success,
                        Value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions)
                    };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Errore durante il recupero valore: {Key}", key);
                throw new RedisCacheException($"Recupero chiave '{key}' fallito", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                await _database.KeyDeleteAsync(key);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                throw new RedisCacheException($"Rimozione chiave '{key}' fallita", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _keyValidator.Validate(key);

                var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration ?? options?.SlidingExpiration;

                await _database.StringSetAsync(key, serializedValue, expiry);
                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Errore durante il salvataggio valore: {Key}", key);
                throw new RedisCacheException($"Salvataggio chiave '{key}' fallito", ex);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
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