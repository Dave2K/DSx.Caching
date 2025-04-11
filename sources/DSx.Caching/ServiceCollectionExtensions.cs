using DSx.Caching.Abstractions.Factories;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DSx.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredCacheProviders(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging();

            // Configurazione obbligatoria per Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["CacheSettings:Redis:ConnectionString"];
                options.InstanceName = configuration["CacheSettings:Redis:InstanceName"];
            });

            // Registrazione esplicita delle factory
            services.AddSingleton<IConnectionMultiplexerFactory, RedisConnectionMultiplexerFactory>();

            return services
                .AddSingleton<CacheProviderFactory>()
                .AddSingleton<ICacheProvider>(sp => sp.GetRequiredService<CacheProviderFactory>().GetProvider());
        }
    }
}