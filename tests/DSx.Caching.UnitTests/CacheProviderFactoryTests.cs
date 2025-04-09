using DSx.Caching;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Memory;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DSx.Caching.Tests
{
    /// <summary>
    /// Test per la CacheProviderFactory
    /// </summary>
    public class CacheProviderFactoryTests
    {
        /// <summary>
        /// Verifica che venga restituito il provider corretto
        /// </summary>
        [Theory]
        [InlineData("Redis", typeof(RedisCacheProvider))]
        [InlineData("MemoryCache", typeof(MemoryCacheProvider))]
        public void GetProvider_WithValidName_ReturnsCorrectType(string providerName, Type expectedType)
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
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
                .AddConfiguredCacheProviders(config)
                .BuildServiceProvider();

            var factory = new CacheProviderFactory(config, services);

            // Act
            var provider = factory.GetProvider(providerName);

            // Assert
            Assert.IsType(expectedType, provider);
        }

        /// <summary>
        /// Verifica che venga generata un'eccezione per provider non configurati
        /// </summary>
        [Fact]
        public void GetProvider_WithUnconfiguredProvider_ThrowsException()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["CacheSettings:Providers:0"] = "MemoryCache"
                })
                .Build();

            var services = new ServiceCollection()
                .AddMemoryCache()
                .BuildServiceProvider();

            var factory = new CacheProviderFactory(config, services);

            // Act & Assert
            Assert.Throws<ProviderNotConfiguredException>(() => factory.GetProvider("Redis"));
        }
    }
}