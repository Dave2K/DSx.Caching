using DSx.Caching;
using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Factories;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.UnitTests
{
    /// <summary>
    /// Test per la CacheProviderFactory
    /// </summary>
    public class CacheProviderFactoryTests
    {
        /// <summary>
        /// Verifica la creazione corretta di un provider Redis
        /// </summary>
        [Fact]
        public void GetProvider_Redis_CreaIstanzaCorretta()
        {
            // Configurazione
            var inMemorySettings = new Dictionary<string, string>
            {
                ["CacheSettings:DefaultProvider"] = "Redis",
                ["CacheSettings:Redis:ConnectionString"] = "localhost:6379",
                ["CacheSettings:Redis:InstanceName"] = "TestInstance"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration)
                .AddSingleton<ICacheKeyValidator, CacheKeyValidator>()
                .AddSingleton(Options.Create(new JsonSerializerOptions()))
                .AddSingleton<IConnectionMultiplexerFactory, RedisConnectionMultiplexerFactory>();

            var serviceProvider = services.BuildServiceProvider();

            // Esecuzione
            var factory = new CacheProviderFactory(configuration, serviceProvider);
            var provider = factory.GetProvider("Redis");

            // Verifica
            Assert.IsType<RedisCacheProvider>(provider);
        }

        /// <summary>
        /// Verifica il comportamento con un provider non configurato
        /// </summary>
        [Fact]
        public void GetProvider_ConProviderNonConfigurato_GeneraEccezione()
        {
            // Configurazione
            var configuration = new ConfigurationBuilder().Build();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            // Esecuzione & Verifica
            var factory = new CacheProviderFactory(configuration, serviceProvider);
            Assert.Throws<ProviderNotConfiguredException>(() => factory.GetProvider("InMemory"));
        }
    }
}