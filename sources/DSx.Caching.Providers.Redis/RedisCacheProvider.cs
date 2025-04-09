using DSx.Caching.Abstractions.Exceptions;
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
    /// Fornisce un'implementazione Redis per il caching dei dati
    /// </summary>
    public sealed class RedisCacheProvider : ICacheProvider, IAsyncDisposable, IDisposable
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
        /// <param name="logger">Logger per la tracciatura delle operazioni</param>
        /// <param name="keyValidator">Validatore per le chiavi di cache</param>
        /// <param name="jsonOptions">Opzioni di serializzazione JSON</param>
        /// <exception cref="ArgumentNullException">Generata se un parametro richiesto è null</exception>
        public RedisCacheProvider(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValidator = keyValidator ?? throw new ArgumentNullException(nameof(keyValidator));
            _serializerOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            _database = _connection.GetDatabase();
        }

        /// <inheritdoc/>
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
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operazione annullata per la chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
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
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var redisValue = await _database.StringGetAsync(key);

                if (redisValue.IsNullOrEmpty)
                {
                    return new CacheOperationResult<T>
                    {
                        Status = CacheOperationStatus.NotFound
                    };
                }

                var value = JsonSerializer.Deserialize<T>(redisValue.ToString(), _serializerOptions);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.Success,
                    Value = value
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Errore deserializzazione per chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Formato dati non valido"
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Lettura annullata per chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generico durante la lettura chiave: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
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
                _keyValidator.Validate(key);
                cancellationToken.ThrowIfCancellationRequested();

                var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
                var expiry = options?.AbsoluteExpiration ?? options?.SlidingExpiration;

                await _database.StringSetAsync(key, serializedValue, expiry);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Errore serializzazione per chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Formato dati non valido"
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Scrittura annullata per chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
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
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Rimozione annullata per chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione chiave: {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
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

                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.Success
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Pulizia globale annullata");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = "Operazione annullata"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la pulizia globale della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        private void Dispose(bool disposing)
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

        /// <inheritdoc/>
        private async ValueTask DisposeAsyncCore()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await _connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}