using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Providers.Redis.HealthChecks
{
    /// <summary>
    /// Fornisce controlli sullo stato della connessione Redis
    /// </summary>
    public sealed class RedisCacheHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connection;

        /// <summary>
        /// Costruttore principale
        /// </summary>
        /// <param name="connection">Istanza della connessione Redis</param>
        public RedisCacheHealthCheck(IConnectionMultiplexer connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Esegue il controllo dello stato della connessione Redis
        /// </summary>
        /// <param name="context">Contesto del controllo</param>
        /// <param name="cancellationToken">Token per annullamento operazione</param>
        /// <returns>Risultato del controllo dello stato</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_connection.IsConnected)
                    return HealthCheckResult.Unhealthy("Disconnesso", data: GetMetrics());

                var db = _connection.GetDatabase();
                var latency = await db.PingAsync().ConfigureAwait(false);

                var metrics = GetMetrics();
                metrics["latency_ms"] = latency.TotalMilliseconds;

                return latency.TotalMilliseconds > 1000
                    ? HealthCheckResult.Degraded($"Latenza elevata: {latency.TotalMilliseconds}ms", data: metrics)
                    : HealthCheckResult.Healthy("Operativo", metrics);
            }
            catch (RedisException ex)
            {
                return HealthCheckResult.Unhealthy("Errore critico Redis", ex, GetMetrics());
            }
        }

        /// <summary>
        /// Raccolta delle metriche di sistema Redis
        /// </summary>
        /// <returns>Dizionario con le metriche raccolte</returns>
        private Dictionary<string, object> GetMetrics()
        {
            var data = new Dictionary<string, object>
            {
                ["server_count"] = _connection.GetEndPoints().Length,
                ["config"] = _connection.Configuration
            };

            foreach (var endpoint in _connection.GetEndPoints())
            {
                var server = _connection.GetServer(endpoint);
                data[$"server_{endpoint}"] = new
                {
                    server.ServerType,
                    server.Version,
                    Status = server.IsConnected ? "OK" : "Errore connessione"
                };
            }

            return data;
        }
    }
}