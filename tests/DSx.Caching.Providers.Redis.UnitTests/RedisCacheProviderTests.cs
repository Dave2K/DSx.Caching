using DSx.Caching.Abstractions.Models;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Test unitari per RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ILogger<RedisCacheProvider>> _mockLogger;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore che inizializza i mock
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<RedisCacheProvider>>();

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                "localhost",
                _mockLogger.Object,
                _mockConnection.Object,
                new JsonSerializerOptions());
        }

        /// <summary>
        /// Verifica che ExistsAsync restituisca Success quando la chiave esiste
        /// </summary>
        [Fact]
        public async Task ExistsAsync_ChiaveEsistente_RestituisceSuccess()
        {
            // Arrange
            const string testKey = "chiave_test";
            _mockDatabase.Setup(db => db.KeyExistsAsync(testKey, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var result = await _provider.ExistsAsync(testKey);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
        }

        /// <summary>
        /// Verifica che Dispose chiuda correttamente la connessione
        /// </summary>
        [Fact]
        public void Dispose_ChiudeConnessione()
        {
            // Act
            _provider.Dispose();

            // Assert
            _mockConnection.Verify(c => c.Close(true), Times.Once);
        }

        /// <summary>
        /// Implementazione di IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Metodo protetto per la pulizia delle risorse
        /// </summary>
        /// <param name="disposing">True per rilasciare risorse managed e unmanaged</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _provider.Dispose();
                }
                _disposed = true;
            }
        }
    }
}