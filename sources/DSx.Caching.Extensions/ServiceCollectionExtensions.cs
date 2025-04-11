using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Memory;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DSx.Caching.Extensions
{
    /// <summary>
    /// Estensioni per la configurazione del core del caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Aggiunge i servizi core del caching
        /// </summary>
        public static IServiceCollection AddCachingCore(this IServiceCollection services)
        {
            services.AddSingleton<ICacheKeyValidator, CacheKeyValidator>();
            return services;
        }

        /// <summary>
        /// Aggiunge il provider MemoryCache
        /// </summary>
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
        {
            return services
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<ICacheProvider, MemoryCacheProvider>();
        }
    }
}