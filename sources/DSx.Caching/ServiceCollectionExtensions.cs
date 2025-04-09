using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Interfaces;

namespace DSx.Caching
{
    /// <summary>
    /// Estensioni per la configurazione dei servizi di caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra i provider di cache configurati
        /// </summary>
        /// <param name="services">Collezione dei servizi</param>
        /// <param name="configuration">Configurazione dell'applicazione</param>
        /// <returns>Riferimento alla collezione dei servizi</returns>
        /// <exception cref="ArgumentNullException">Se mancano configurazioni essenziali</exception>
        public static IServiceCollection AddConfiguredCacheProviders(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging();

            var cacheSettings = configuration.GetSection("CacheSettings");
            var providers = cacheSettings.GetSection("Providers").Get<string[]>() ?? [];

            // Configurazione Redis
            if (providers.Contains("Redis", StringComparer.OrdinalIgnoreCase))
            {
                var redisConfig = cacheSettings.GetSection("Redis");
                var connectionString = redisConfig["ConnectionString"]
                    ?? throw new ArgumentNullException(nameof(configuration), "Redis:ConnectionString mancante");

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = connectionString;
                    options.InstanceName = redisConfig["InstanceName"];
                });

                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(connectionString));
            }

            // Configurazione MemoryCache
            if (providers.Contains("MemoryCache", StringComparer.OrdinalIgnoreCase))
            {
                var memoryConfig = cacheSettings.GetSection("MemoryCache");
                services.AddMemoryCache(options =>
                {
                    options.SizeLimit = memoryConfig.GetValue<long?>("SizeLimit");
                    options.CompactionPercentage = memoryConfig.GetValue<double>("CompactionPercentage");
                    options.ExpirationScanFrequency = memoryConfig.GetValue<TimeSpan>("ExpirationScanFrequency");
                });
            }

            return services
                .AddSingleton<CacheProviderFactory>()
                .AddSingleton<ICacheProvider>(sp => sp.GetRequiredService<CacheProviderFactory>().GetProvider());
        }
    }
}