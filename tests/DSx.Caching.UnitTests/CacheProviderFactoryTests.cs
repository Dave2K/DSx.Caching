using DSx.Caching;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.UnitTests
{
    /// <summary>
    /// Classe di test per verificare il corretto funzionamento di RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ILogger<RedisCacheProvider>> _mockLogger;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly IOptions<JsonSerializerOptions> _jsonOptions;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore che inizializza l'ambiente di test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<RedisCacheProvider>>();
            _keyValidator = new CacheKeyValidator();
            _jsonOptions = Options.Create(new JsonSerializerOptions());

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _keyValidator,
                _jsonOptions);
        }

        /// <summary>
        /// Verifica che GetAsync restituisca Success quando la chiave esiste
        /// </summary>
        /// <returns>Task asincrono</returns>
        [Fact]
        public async Task GetAsync_ConChiaveValida_RestituisceValore()
        {
            // Arrange
            const string testKey = "test_key";
            const string testValue = "test_value";

            _mockDatabase.Setup(db => db.StringGetAsync(testKey, CommandFlags.None))
                .ReturnsAsync(testValue);

            // Act
            var result = await _provider.GetAsync<string>(testKey);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal(testValue, result.Value);
        }

        /// <summary>
        /// Verifica che SetAsync memorizzi correttamente un valore e restituisca Success
        /// </summary>
        /// <returns>Task asincrono</returns>
        [Fact]
        public async Task SetAsync_ConDatiValidi_MemorizzaValore()
        {
            // Arrange
            const string testKey = "test_key";
            const string testValue = "test_value";

            _mockDatabase.Setup(db => db.StringSetAsync(
                testKey,
                It.IsAny<RedisValue>(),
                null,
                When.Always,
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var result = await _provider.SetAsync(testKey, testValue);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);

            _mockDatabase.Verify(db => db.StringSetAsync(
                testKey,
                It.Is<RedisValue>(v => v.ToString() == testValue),
                null,
                When.Always,
                CommandFlags.None),
                Times.Once);
        }

        /// <summary>
        /// Implementazione di IDisposable per la pulizia delle risorse
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _provider?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}