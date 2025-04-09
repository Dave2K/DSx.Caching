using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Core.UnitTests
{
    /// <summary>
    /// Test suite per la classe <see cref="BaseCacheManager"/>
    /// </summary>
    public class BaseCacheManagerTests : IDisposable
    {
        private readonly Mock<ICacheProvider> _mockProvider;
        private readonly TestCacheManager _manager;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Inizializza una nuova istanza della classe di test
        /// </summary>
        public BaseCacheManagerTests()
        {
            _mockProvider = new Mock<ICacheProvider>();
            _loggerFactory = LoggerFactory.Create(builder => { });

            _manager = new TestCacheManager(
                _loggerFactory.CreateLogger<TestCacheManager>(),
                _mockProvider.Object
            );
        }

        /// <summary>
        /// Verifica che gli errori vengano gestiti correttamente nel metodo GetInternalAsync
        /// </summary>
        [Fact]
        public async Task GetInternalAsync_inCasoDiErrore_deveRestituireValidationError()
        {
            // Arrange
            const string testKey = "test_key";
            var expectedError = new Exception("Errore simulato");

            _mockProvider.Setup(p => p.GetAsync<int>(
                testKey,
                It.IsAny<CacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedError);

            // Act
            var result = await _manager.PublicGetInternalAsync(testKey);

            // Assert
            Assert.Equal(CacheOperationStatus.ValidationError, result.Status);
            Assert.Contains(expectedError.Message, result.Details);
        }

        /// <summary>
        /// Rilascia le risorse allocate per i test
        /// </summary>
        public void Dispose()
        {
            _loggerFactory?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione concreta di BaseCacheManager per scopi di test
        /// </summary>
        /// <param name="Logger">Istanza del logger</param>
        /// <param name="CacheProvider">Provider della cache</param>
        public sealed class TestCacheManager(
            ILogger<TestCacheManager> Logger,
            ICacheProvider CacheProvider) : BaseCacheManager(Logger, CacheProvider)
        {
            /// <summary>
            /// Espone il metodo interno per il testing
            /// </summary>
            /// <param name="key">Chiave da recuperare</param>
            /// <returns>Risultato dell'operazione</returns>
            public Task<CacheOperationResult<int>> PublicGetInternalAsync(string key)
            {
                return GetInternalAsync(
                    key,
                    () => CacheProvider.GetAsync<int>(
                        key,
                        null,
                        CancellationToken.None));
            }
        }
    }
}