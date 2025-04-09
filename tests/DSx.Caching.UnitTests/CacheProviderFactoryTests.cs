using System;
using System.Collections.Generic;
using DSx.Caching;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Memory;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DSx.Caching.UnitTests
{
    /// <summary>
    /// Classe di test per verificare il corretto funzionamento di CacheProviderFactory
    /// </summary>
    public class CacheProviderFactoryTests
    {
        /// <summary>
        /// Verifica che venga restituito il tipo corretto di provider in base al nome
        /// </summary>
        /// <param name="providerName">Nome del provider da testare</param>
        /// <param name="expectedType">Tipo atteso del provider</param>
        [Theory]
        [InlineData("Redis", typeof(RedisCacheProvider))]
        [InlineData("MemoryCache", typeof(MemoryCacheProvider))]
        public void GetProvider_WithValidName_ReturnsCorrectType(string providerName, Type expectedType)
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["CacheSettings:Providers:0"] = "Redis",
                    ["CacheSettings:Providers:1"] = "MemoryCache",
                    ["CacheSettings:DefaultProvider"] = "Redis",
                    ["Redis:ConnectionString"] = "localhost",
                    ["Redis:InstanceName"] = "Test"
                })
                .Build();

            var services = new ServiceCollection()
                .AddLogging()
                .AddMemoryCache()
                .AddConfiguredCacheProviders(config);

            var provider = services.BuildServiceProvider();

            // Act
            var factory = provider.GetRequiredService<CacheProviderFactory>();
            var result = factory.GetProvider(providerName);

            // Assert
            Assert.IsType(expectedType, result);
        }
    }
}