using DSx.Caching.Abstractions.Clustering;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Client per la gestione del cluster Redis e la sincronizzazione tra nodi
    /// </summary>
    public sealed class RedisCacheClusterClient : ICacheClusterClient, IDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisCacheClusterClient> _logger;
        private bool _disposed;

        private const string InvalidationChannel = "cache-invalidation";
        private const string SyncChannel = "cluster-sync";

        /// <summary>
        /// Inizializza una nuova istanza del client per cluster Redis
        /// </summary>
        /// <param name="connection">Connessione multiplexer Redis</param>
        /// <param name="logger">Logger per tracciatura attività</param>
        public RedisCacheClusterClient(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheClusterClient> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriber = _connection.GetSubscriber();
        }

        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster
        /// </summary>
        /// <param name="key">Chiave da invalidare</param>
        /// <returns>Task asincrono</returns>
        public async Task BroadcastInvalidationAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Chiave non valida", nameof(key));

            await _subscriber.PublishAsync(
                new RedisChannel(InvalidationChannel, RedisChannel.PatternMode.Literal),
                key
            ).ConfigureAwait(false);

            _logger.LogDebug("Invalidato {Key} su tutto il cluster", key);
        }

        /// <summary>
        /// Invalida tutte le chiavi che corrispondono a un pattern
        /// </summary>
        /// <param name="pattern">Pattern di ricerca per le chiavi</param>
        /// <returns>Task asincrono</returns>
        public async Task InvalidateByPatternAsync(string pattern)
        {
            var endpoints = _connection.GetEndPoints(configuredOnly: true);

            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    await _connection.GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Invalidato pattern: {Pattern}", pattern);
        }

        /// <summary>
        /// Sincronizza lo stato del cluster Redis
        /// </summary>
        /// <returns>Task asincrono</returns>
        public async Task SyncClusterStateAsync()
        {
            var db = _connection.GetDatabase();
            await db.ExecuteAsync("CLUSTER", "FORGET").ConfigureAwait(false);
            _logger.LogInformation("Stato cluster sincronizzato");
        }

        /// <summary>
        /// Rilascia le risorse del client
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _subscriber.UnsubscribeAll();
            _connection.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}