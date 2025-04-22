using DSx.Caching.Abstractions.Clustering;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Redis;
using DSx.Caching.Providers.Redis.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DSx.Caching.DependencyInjection
{
    /// <summary>
    /// Metodi di estensione per la registrazione dei servizi Redis
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Aggiunge il provider Redis al sistema di dependency injection
        /// </summary>
        /// <param name="services">Collezione di servizi</param>
        /// <param name="configurationString">Stringa di configurazione Redis</param>
        /// <returns>Collezione di servizi aggiornata</returns>
        public static IServiceCollection AddRedisCacheProvider(
            this IServiceCollection services,
            string configurationString)
        {
            return services
                .AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(configurationString))
                .AddSingleton<ICacheClusterClient, RedisCacheClusterClient>()
                .AddSingleton<ICacheProvider, RedisCacheProvider>()
                .AddHealthChecks()
                .AddCheck<RedisCacheHealthCheck>(
                    name: "RedisCacheHealthCheck",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy
                )
                .Services;
        }
    }
}