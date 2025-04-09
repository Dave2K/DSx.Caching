using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Core.UnitTests;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Test unitari per RedisCacheProvider
    /// </summary>
    /// <remarks>
    /// Verifica tutte le interazioni con Redis tramite mock
    /// </remarks>
    public class RedisCacheProviderTests : ProviderContractTests, IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly ConcurrentDictionary<string, string> _fakeRedisStore;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore per l'inizializzazione dei test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _fakeRedisStore = new ConcurrentDictionary<string, string>();

            // Configurazione mock per SetAsync
            _mockDatabase
                .Setup(db => db.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    It.IsAny<bool>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()))
                .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                    (key, value, _, _, _, _) => _fakeRedisStore.TryAdd((string)key!, value.ToString())
                )
                .ReturnsAsync(true);

            // Configurazione mock per GetAsync
            _mockDatabase
                .Setup(db => db.StringGetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync((RedisKey key, CommandFlags _) =>
                    _fakeRedisStore.TryGetValue((string)key!, out var val) ? val : RedisValue.Null);

            // Configurazione mock per DeleteAsync
            _mockDatabase
                .Setup(db => db.KeyDeleteAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<CommandFlags>()))
                .Callback<RedisKey, CommandFlags>(
                    (key, _) => _fakeRedisStore.TryRemove((string)key!, out string? _) // Fix esplicito del tipo
                )
                .ReturnsAsync(true);

            _mockConnection.Setup(c => c.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object>())).Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                Mock.Of<ILogger<RedisCacheProvider>>(),
                new CacheKeyValidator(),
                new JsonSerializerOptions());
        }

        /// <summary>
        /// Crea un'istanza del provider per i test
        /// </summary>
        protected override ICacheProvider CreateProvider() => _provider;

        /// <summary>
        /// Pulisce le risorse dopo i test
        /// </summary>
        protected override void CleanupProvider() => Dispose();

        /// <summary>
        /// Verifica l'inserimento e recupero di un valore
        /// </summary>
        [Fact]
        public async Task Redis_SetAndGet_ValidKey_ReturnsCorrectValue()
        {
            // Arrange
            const string key = "test_key";
            const string value = "test_value";

            // Act
            await _provider.SetAsync(key, value);
            var result = await _provider.GetAsync<string>(key);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal(value, result.Value);
        }

        /// <summary>
        /// Verifica la rimozione corretta di una chiave
        /// </summary>
        [Fact]
        public async Task Redis_Remove_ExistingKey_RemovesSuccessfully()
        {
            // Arrange
            const string key = "key_to_remove";
            await _provider.SetAsync(key, "value");

            // Act
            var result = await _provider.RemoveAsync(key);
            var getResult = await _provider.GetAsync<string>(key);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal(CacheOperationStatus.NotFound, getResult.Status);
        }

        /// <summary>
        /// Rilascia le risorse del test
        /// </summary>
        public new void Dispose()
        {
            if (!_disposed)
            {
                _provider.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}