using DSx.Caching.Abstractions.Models;
using DSx.Caching.Providers.Redis;
using DSx.Caching.SharedKernel.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Classe di test per verificare il comportamento del RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection = new();
        private readonly Mock<IDatabase> _mockDb = new();
        private readonly Mock<ILogger<RedisCacheProvider>> _mockLogger = new();
        private readonly Mock<ICacheKeyValidator> _mockValidator = new();
        private readonly JsonSerializerOptions _serializerOptions = new();
        private const string TestKey = "valid_key";
        private const string TestValue = "test_value";

        /// <summary>
        /// Costruttore che inizializza i mock per il test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDb.Object);
        }

        /// <summary>
        /// Verifica che il metodo GetAsync restituisca Success quando la chiave esiste
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnSuccess_WhenKeyExists()
        {
            // Arrange
            var serialized = JsonSerializer.SerializeToUtf8Bytes(TestValue);
            _mockDb.Setup(x => x.StringGetAsync(TestKey, CommandFlags.None))
                .ReturnsAsync(serialized);

            var provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _serializerOptions);

            // Act
            var result = await provider.GetAsync<string>(TestKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be(TestValue);
            result.Details.Should().BeNull();
        }

        /// <summary>
        /// Verifica che il metodo GetAsync restituisca NotFound quando la chiave non esiste
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnNotFound_WhenKeyDoesNotExist()
        {
            // Arrange
            _mockDb.Setup(x => x.StringGetAsync(TestKey, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            var provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _serializerOptions);

            // Act
            var result = await provider.GetAsync<string>(TestKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.NotFound);
            result.Value.Should().BeNull();
            result.Details.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Verifica che il metodo GetAsync restituisca ConnectionError in caso di eccezione Redis
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnConnectionError_OnRedisException()
        {
            // Arrange
            _mockDb.Setup(x => x.StringGetAsync(TestKey, CommandFlags.None))
                .ThrowsAsync(new RedisException("Connection failed"));

            var provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _serializerOptions);

            // Act
            var result = await provider.GetAsync<string>(TestKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ConnectionError);
            result.Value.Should().BeNull();
            result.Details.Should().Contain("Connection failed");
        }
    }
}
