using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="RedisCacheProvider"/>
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ILogger<RedisCacheProvider>> _mockLogger;
        private readonly Mock<ICacheKeyValidator> _mockKeyValidator;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Inizializza i componenti per i test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<RedisCacheProvider>>();
            _mockKeyValidator = new Mock<ICacheKeyValidator>();

            _mockConnection.Setup(c => c.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _mockKeyValidator.Object,
                new JsonSerializerOptions());
        }

        /// <summary>
        /// Verifica la corretta chiusura della connessione Redis
        /// </summary>
        [Fact]
        public void Dispose_ChiudeCorrettamenteLaConnessione()
        {
            // Act
            _provider.Dispose();

            // Assert
            _mockConnection.Verify(c => c.Close(true), Times.Once);
            _mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        /// <summary>
        /// Verifica il comportamento con chiavi esistenti
        /// </summary>
        [Fact]
        public async Task ExistsAsync_ChiaveEsistente_RestituisceSuccesso()
        {
            // Arrange
            const string testKey = "chiave_test";
            _mockDatabase.Setup(db => db.KeyExistsAsync(testKey, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var result = await _provider.ExistsAsync(testKey);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            _mockDatabase.Verify(db => db.KeyExistsAsync(testKey, CommandFlags.None), Times.Once);
        }

        /// <summary>
        /// Verifica il comportamento con errore di connessione
        /// </summary>
        [Fact]
        public async Task GetAsync_ErroreDiConnessione_GeneraEccezione()
        {
            // Arrange
            const string testKey = "chiave_invalida";
            _mockDatabase.Setup(db => db.StringGetAsync(testKey, CommandFlags.None))
                .ThrowsAsync(new RedisConnectionException(
                    ConnectionFailureType.UnableToResolvePhysicalConnection,
                    "Errore simulato"));

            // Act & Assert
            await Assert.ThrowsAsync<RedisCacheException>(() =>
                _provider.GetAsync<string>(testKey));
        }

        /// <summary>
        /// Verifica la corretta serializzazione JSON
        /// </summary>
        [Fact]
        public async Task SetAsync_SerializzaCorrettamenteIlValore()
        {
            // Arrange
            const string testKey = "oggetto_complesso";
            var testValue = new { Nome = "Test", Valore = 123 };
            var expectedJson = JsonSerializer.Serialize(testValue);

            // Act
            await _provider.SetAsync(testKey, testValue);

            // Assert
            _mockDatabase.Verify(db =>
                db.StringSetAsync(
                    testKey,
                    It.Is<RedisValue>(v => v.ToString() == expectedJson),
                    null,
                    false,
                    When.Always,
                    CommandFlags.None),
                Times.Once);
        }

        /// <summary>
        /// Verifica la gestione della scadenza assoluta
        /// </summary>
        [Fact]
        public async Task SetAsync_ImpostaScadenzaCorretta()
        {
            // Arrange
            const string testKey = "key_con_scadenza";
            var options = new CacheEntryOptions { AbsoluteExpiration = TimeSpan.FromMinutes(30) };

            // Act
            await _provider.SetAsync(testKey, "valore", options);

            // Assert
            _mockDatabase.Verify(db =>
                db.StringSetAsync(
                    testKey,
                    It.IsAny<RedisValue>(),
                    options.AbsoluteExpiration,
                    false,
                    When.Always,
                    CommandFlags.None),
                Times.Once);
        }

        /// <summary>
        /// Verifica la rimozione corretta di una chiave
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ChiaveEsistente_RimozioneRiuscita()
        {
            // Arrange
            const string testKey = "chiave_da_rimuovere";
            _mockDatabase.Setup(db => db.KeyDeleteAsync(testKey, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var result = await _provider.RemoveAsync(testKey);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            _mockDatabase.Verify(db => db.KeyDeleteAsync(testKey, CommandFlags.None), Times.Once);
        }

        /// <summary>
        /// Verifica la pulizia completa della cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_SvuotaTutteLeDatabase()
        {
            // Act
            await _provider.ClearAllAsync();

            // Assert
            _mockConnection.Verify(c => c.GetEndPoints(It.IsAny<bool>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Gestione della pulizia delle risorse
        /// </summary>
        public void Dispose()
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