using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DSx.Caching
{
    /// <summary>
    /// Estensioni per la configurazione dei servizi di caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Aggiunge i provider di cache configurati tramite IConfiguration
        /// </summary>
        /// <param name="services">Collezione dei servizi</param>
        /// <param name="configuration">Configurazione dell'applicazione</param>
        /// <returns>Riferimento alla collezione dei servizi</returns>
        /// <exception cref="ArgumentNullException">Se la configurazione è mancante</exception>
        public static IServiceCollection AddConfiguredCacheProviders(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging();

            var cacheSettings = configuration.GetSection("CacheSettings");
            var providers = cacheSettings.GetSection("Providers").Get<string[]>() ?? Array.Empty<string>();

            if (providers.Contains("Redis"))
            {
                var redisConfig = cacheSettings.GetSection("Redis");
                var connectionString = redisConfig["ConnectionString"] 
                    ?? throw new ArgumentNullException(nameof(configuration), "Redis:ConnectionString non configurato");

                services.AddStackExchangeRedisCache(options => 
                {
                    options.Configuration = connectionString;
                    options.InstanceName = redisConfig["InstanceName"];
                });

                services.AddSingleton<IConnectionMultiplexer>(_ => 
                    ConnectionMultiplexer.Connect(connectionString));
            }

            if (providers.Contains("MemoryCache"))
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