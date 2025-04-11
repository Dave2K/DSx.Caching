// File: tests/DSx.Caching.UnitTests/CacheProviderFactoryTests.cs
using DSx.Caching;
using DSx.Caching.Providers.Redis;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.UnitTests
{
    public class CacheProviderFactoryTests
    {
        [Fact]
        public void GetProvider_Redis_CreaIstanzaCorretta()
        {
            // Configurazione completa e corretta
            var inMemorySettings = new Dictionary<string, string?>
            {
                ["CacheSettings:DefaultProvider"] = "Redis",
                ["CacheSettings:Providers"] = "Redis", // Formato array corretto
                ["CacheSettings:Redis:ConnectionString"] = "localhost:6379",
                ["CacheSettings:Redis:InstanceName"] = "TestInstance",
                ["CacheSettings:Redis:OperationTimeoutMs"] = "5000"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var services = new ServiceCollection();

            // Registrazione completa dei servizi necessari
            services.AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["CacheSettings:Redis:ConnectionString"];
                    options.InstanceName = configuration["CacheSettings:Redis:InstanceName"];
                })
                .AddSingleton<RedisConnectionMultiplexerFactory>()
                .AddSingleton<ICacheKeyValidator, CacheKeyValidator>()
                .AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions());

            var serviceProvider = services.BuildServiceProvider();

            var factory = new CacheProviderFactory(configuration, serviceProvider);

            // Verifica risoluzione provider
            var provider = factory.GetProvider("Redis");
            Assert.IsType<RedisCacheProvider>(provider);
        }
    }
}