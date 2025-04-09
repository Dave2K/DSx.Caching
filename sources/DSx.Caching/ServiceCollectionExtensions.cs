using DSx.Caching.Abstractions.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DSx.Caching
{
    /// <summary>
    /// Estensioni per la configurazione dei servizi di caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Aggiunge i provider di cache configurati
        /// </summary>
        /// <param name="services">Collezione di servizi</param>
        /// <param name="configuration">Configurazione dell'applicazione</param>
        public static void AddConfiguredCacheProviders(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var cacheSettings = configuration.GetSection("CacheSettings");
            var providers = cacheSettings.GetSection("Providers").Get<string[]>();

            // Registra Redis solo se configurato
            if (providers?.Contains("Redis") == true)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings["Redis:ConnectionString"];
                    options.InstanceName = cacheSettings["Redis:InstanceName"];
                });
            }

            // Registra MemoryCache con opzioni
            if (providers?.Contains("MemoryCache") == true)
            {
                services.AddMemoryCache(options =>
                {
                    options.SizeLimit = cacheSettings.GetValue<long?>("MemoryCache:SizeLimit");
                });
            }

            // Registra la factory e il provider di default
            services.AddSingleton<CacheProviderFactory>();
            services.AddSingleton<ICacheProvider>(sp =>
                sp.GetRequiredService<CacheProviderFactory>().GetProvider());
        }
    }
}