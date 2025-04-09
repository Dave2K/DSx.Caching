using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Suite di test per il RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza della classe di test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();

            var mockLogger = new Mock<ILogger<RedisCacheProvider>>();
            var keyValidator = new CacheKeyValidator();
            var jsonOptions = Options.Create(new JsonSerializerOptions());

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                mockLogger.Object,
                keyValidator,
                jsonOptions);
        }

        /// <summary>
        /// Verifica che il recupero con chiave non valida generi un'eccezione
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveNonValida_GeneraEccezione()
        {
            // Arrange
            var mockConnection = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            var mockValidator = new Mock<ICacheKeyValidator>();

            // Configura il validatore per simulare una chiave valida
            mockValidator.Setup(v => v.Validate(It.IsAny<string>())).Verifiable();

            // Use RedisKey instead of string for the mock setup
            mockDatabase
                .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ThrowsAsync(new RedisConnectionException(
                    ConnectionFailureType.UnableToResolvePhysicalConnection,
                    "Errore simulato"
                ));

            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(mockDatabase.Object);

            var provider = new RedisCacheProvider(
                mockConnection.Object,
                Mock.Of<ILogger<RedisCacheProvider>>(),
                mockValidator.Object,
                Options.Create(new JsonSerializerOptions())
            );

            // Act & Assert
            await Assert.ThrowsAsync<RedisConnectionException>(
                () => provider.GetAsync<string>("any_key")
            );

            // Verifica che il validatore sia stato chiamato
            mockValidator.Verify();
        }

        /// <summary>
        /// Verifica il corretto funzionamento del metodo Dispose
        /// </summary>
        [Fact]
        public void Dispose_ChiudeConnessioneCorrettamente()
        {
            // Act
            _provider.Dispose();

            // Assert
            _mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        /// <summary>
        /// Pulisce le risorse dopo ogni test
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