using StackExchange.Redis;
using DSx.Caching.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Client per la gestione di operazioni cluster su Redis.
    /// Implementa meccanismi di invalidazione distribuita e sincronizzazione dello stato del cluster.
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
        /// Inizializza una nuova istanza della classe RedisCacheClusterClient.
        /// </summary>
        /// <param name="connection">Connessione multiplexer a Redis.</param>
        /// <param name="logger">Logger per tracciare le operazioni.</param>
        /// <exception cref="ArgumentNullException">Se connection o logger sono null.</exception>
        public RedisCacheClusterClient(
            IConnectionMultiplexer connection,
            ILogger<RedisCacheClusterClient> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriber = _connection.GetSubscriber();
        }

        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster.
        /// </summary>
        /// <param name="key">Chiave da invalidare.</param>
        /// <returns>Task asincrono.</returns>
        /// <exception cref="ArgumentException">Se la chiave è vuota o non valida.</exception>
        public async Task BroadcastInvalidationAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogError("Tentativo di invalidare una chiave vuota o nulla");
                throw new ArgumentException("La chiave non può essere vuota", nameof(key));
            }

            try
            {
                await _subscriber.PublishAsync(
                    new RedisChannel(InvalidationChannel, RedisChannel.PatternMode.Literal),
                    key
                ).ConfigureAwait(false);

                _logger.LogInformation("Invalidata chiave {Key} sul cluster Redis", key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(
                    ex,
                    "Errore Redis durante l'invalidazione della chiave {Key}. Dettagli: {ErrorMessage}",
                    key,
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Invalida tutte le chiavi che corrispondono a un pattern specificato.
        /// </summary>
        /// <param name="pattern">Pattern per la ricerca delle chiavi (es. 'prefix:*').</param>
        /// <returns>Task asincrono.</returns>
        public async Task InvalidateByPatternAsync(string pattern)
        {
            var endpoints = _connection.GetEndPoints(configuredOnly: true);
            var totalInvalidated = 0;

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var server = _connection.GetServer(endpoint);
                    var keys = new List<RedisKey>();

                    await foreach (var key in server.KeysAsync(pattern: pattern).ConfigureAwait(false))
                    {
                        keys.Add(key);
                    }

                    if (keys.Count > 0)
                    {
                        await _connection.GetDatabase().KeyDeleteAsync(keys.ToArray()).ConfigureAwait(false);
                        totalInvalidated += keys.Count;
                        _logger.LogDebug(
                            "Invalidate {KeyCount} chiavi su endpoint {Endpoint} per pattern {Pattern}",
                            keys.Count,
                            endpoint,
                            pattern
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Fallita invalidazione su endpoint {Endpoint}. Dettagli: {ErrorMessage}",
                        endpoint,
                        ex.Message
                    );
                }
            }

            _logger.LogInformation(
                "Invalidazione completata. Totale chiavi rimosse: {TotalInvalidated}, Pattern: {Pattern}",
                totalInvalidated,
                pattern
            );
        }

        /// <summary>
        /// Sincronizza lo stato del cluster Redis.
        /// </summary>
        public async Task SyncClusterStateAsync()
        {
            try
            {
                var db = _connection.GetDatabase();
                await db.ExecuteAsync("CLUSTER", "FORGET").ConfigureAwait(false);
                _logger.LogInformation("Sincronizzazione stato cluster completata con successo");
            }
            catch (RedisException ex)
            {
                _logger.LogCritical(
                    ex,
                    "Errore critico durante la sincronizzazione del cluster. Dettagli: {ErrorMessage}",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Rilascia le risorse della connessione Redis.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _subscriber.UnsubscribeAll();
                _connection.Dispose();
                _logger.LogInformation("Connessione Redis chiusa correttamente");
            }
            finally
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
