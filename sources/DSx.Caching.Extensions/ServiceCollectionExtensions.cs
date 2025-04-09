using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DSx.Caching.Extensions
{
    /// <summary>
    /// Estensioni per la registrazione dei servizi di caching
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra i servizi core per il sistema di caching
        /// </summary>
        public static IServiceCollection AddCachingCore(this IServiceCollection services)
        {
            services.AddSingleton<ICacheKeyValidator, CacheKeyValidator>();
            return services;
        }

        /// <summary>
        /// Registra il provider di cache in memoria
        /// </summary>
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
            return services;
        }
    }
}