using DSx.Caching.Abstractions.Configurations;
using DSx.Caching.Abstractions.Factories;
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
    /// Fornisce un'implementazione di cache basata su Redis
    /// </summary>
    public sealed class RedisCacheProvider : BaseCacheProvider
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Inizializza una nuova istanza del provider Redis
        /// </summary>
        /// <param name="connectionFactory">Factory per la connessione a Redis</param>
        /// <param name="configuration">Configurazione del provider Redis</param>
        /// <param name="logger">Logger per la tracciatura delle operazioni</param>
        /// <param name="keyValidator">Validatore delle chiavi di cache</param>
        /// <param name="jsonOptions">Opzioni per la serializzazione JSON</param>
        /// <exception cref="ArgumentNullException">Generata quando uno dei parametri essenziali è null</exception>
        public RedisCacheProvider(
            IConnectionMultiplexerFactory connectionFactory,
            IOptions<RedisCacheProviderConfiguration> configuration,
            ILogger<RedisCacheProvider> logger,
            ICacheKeyValidator keyValidator,
            IOptions<JsonSerializerOptions> jsonOptions)
            : base(logger)
        {
            ArgumentNullException.ThrowIfNull(connectionFactory);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(keyValidator);
            ArgumentNullException.ThrowIfNull(jsonOptions);

            _connection = connectionFactory.CreateConnection(configuration.Value.ConnectionString);
            _database = _connection.GetDatabase();
            _keyValidator = keyValidator;
            _serializerOptions = jsonOptions.Value;
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="options">Opzioni della voce di cache (non utilizzate in questa implementazione)</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var exists = await _database.KeyExistsAsync(key).ConfigureAwait(false);
                return new CacheOperationResult
                {
                    Status = exists ? CacheOperationStatus.Success : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la verifica della chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <param name="options">Opzioni della voce di cache (non utilizzate in questa implementazione)</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione con il valore recuperato</returns>
        public override async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

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
                Logger.LogError(ex, "Errore durante il recupero della chiave {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = ex is RedisException ? CacheOperationStatus.ConnectionError
                                                 : CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave da utilizzare</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di scadenza della cache</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
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
                    false,  // keepTtl
                    When.Always,
                    CommandFlags.None
                ).ConfigureAwait(false);

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante il salvataggio della chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = ex is RedisException ? CacheOperationStatus.ConnectionError
                                                 : CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken ct = default)
        {
            try
            {
                _keyValidator.Validate(key);
                ct.ThrowIfCancellationRequested();

                var deleted = await _database.KeyDeleteAsync(key).ConfigureAwait(false);
                return new CacheOperationResult
                {
                    Status = deleted ? CacheOperationStatus.Success
                                      : CacheOperationStatus.NotFound
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante la rimozione della chiave {Key}", key);
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        /// <param name="ct">Token di annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        public override async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                foreach (var endpoint in _connection.GetEndPoints())
                {
                    var server = _connection.GetServer(endpoint);
                    await server.FlushAllDatabasesAsync().ConfigureAwait(false);
                }

                return new CacheOperationResult { Status = CacheOperationStatus.Success };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Errore durante lo svuotamento della cache");
                return new CacheOperationResult
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Rilascia le risorse asincrone
        /// </summary>
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

        /// <summary>
        /// Rilascia le risorse gestite
        /// </summary>
        /// <param name="disposing">Indica se è in corso un dispose esplicito</param>
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

        /// <summary>
        /// Factory per la creazione di connessioni Redis
        /// </summary>
        public class RedisConnectionMultiplexerFactory : IConnectionMultiplexerFactory
        {
            /// <summary>
            /// Crea una nuova connessione Redis
            /// </summary>
            /// <param name="configurationString">Stringa di connessione</param>
            /// <returns>Connessione multiplexer Redis</returns>
            public IConnectionMultiplexer CreateConnection(string configurationString)
            {
                return ConnectionMultiplexer.Connect(configurationString);
            }
        }
    }
}