using DSx.Caching.Abstractions.Exceptions;
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
            const string chiaveNonValida = "chiave_non_valida!";
            _mockDatabase.Setup(db => db.StringGetAsync(chiaveNonValida, CommandFlags.None))
                .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToResolvePhysicalConnection, "Errore simulato"));

            // Act & Assert
            var eccezione = await Assert.ThrowsAsync<RedisConnectionException>(
                () => _provider.GetAsync<string>(chiaveNonValida)
            );

            Assert.NotNull(eccezione);
            Assert.Contains("Errore simulato", eccezione.Message);
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