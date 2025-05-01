using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Redis;
using DSx.Caching.Providers.Redis.HealthChecks;
using DSx.Caching.SharedKernel.Interfaces;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System.Text.Json;

namespace DSx.Caching.DependencyInjection
{
    /// <summary>
    /// Estensione per la registrazione dei servizi Redis nel container DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Aggiunge il provider Redis al sistema di caching con configurazione completa
        /// </summary>
        /// <param name="services">Collezione di servizi</param>
        /// <param name="configurationString">Stringa di connessione a Redis</param>
        /// <param name="jsonOptions">
        /// Opzioni di serializzazione JSON personalizzate (opzionale)
        /// </param>
        /// <returns>Riferimento alla collezione di servizi per il chaining</returns>
        /// <remarks>
        /// Registra i seguenti servizi:
        /// - ConnectionMultiplexer per la connessione a Redis
        /// - Validatore di chiavi cache
        /// - Client per cluster Redis
        /// - Provider Redis per operazioni cache
        /// - Configurazione della serializzazione JSON
        /// - Health check per monitoraggio Redis
        /// </remarks>
        public static IServiceCollection AddRedisCacheProvider(
            this IServiceCollection services,
            string configurationString,
            JsonSerializerOptions? jsonOptions = null)
        {
            return services
                .AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(configurationString))
                .AddSingleton<ICacheKeyValidator, CacheKeyValidator>()
                .AddSingleton<ICacheClusterClient, RedisCacheClusterClient>()
                .AddSingleton<ICacheProvider, RedisCacheProvider>()
                .AddSingleton(jsonOptions ?? new JsonSerializerOptions())
                .AddHealthChecks()
                .AddCheck<RedisCacheHealthCheck>(
                    name: "RedisCacheHealthCheck",
                    failureStatus: HealthStatus.Unhealthy
                )
                .Services;
        }
    }
}
