using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Memory;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DSx.Caching.Extensions
{
    /// <summary>
    /// Estensioni per la registrazione dei servizi di caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra il provider di cache in memoria
        /// </summary>
        /// <param name="services">Collezione dei servizi</param>
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
            return services;
        }

        /// <summary>
        /// Registra il provider Redis con configurazione
        /// </summary>
        /// <param name="services">Collezione dei servizi</param>
        /// <param name="connectionString">Stringa di connessione Redis</param>
        public static IServiceCollection AddRedisCacheProvider(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(connectionString));
            services.AddSingleton<ICacheProvider, RedisCacheProvider>();
            return services;
        }
    }
}