using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Providers.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="MemoryCacheException"/>
    /// </summary>
    public class MemoryCacheExceptionTests : IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<MemoryCacheProvider>> _mockLogger;
        private readonly Mock<ICacheKeyValidator> _mockKeyValidator;
        private readonly MemoryCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Inizializza i componenti per i test
        /// </summary>
        public MemoryCacheExceptionTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<MemoryCacheProvider>>();
            _mockKeyValidator = new Mock<ICacheKeyValidator>();

            _provider = new MemoryCacheProvider(
                _mockCache.Object,
                _mockLogger.Object,
                _mockKeyValidator.Object
            );
        }

        /// <summary>
        /// Verifica la corretta creazione dell'eccezione con parametri
        /// </summary>
        [Fact]
        public void Costruttore_ConParametriValidi_CreaIstanzaCorrettamente()
        {
            // Arrange
            const string messaggio = "Errore test";
            var innerEx = new InvalidOperationException();

            // Act
            var ex = new MemoryCacheException(messaggio, innerEx);

            // Assert
            Assert.Equal(messaggio, ex.Message);
            Assert.Same(innerEx, ex.InnerException);
        }

        /// <summary>
        /// Verifica il logging automatico durante la creazione
        /// </summary>
        [Fact]
        public void Costruttore_ConLogger_RegistraErrore()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MemoryCacheException>>();

            // Act
            var ex = new MemoryCacheException(
                mockLogger.Object,
                "Errore di test",
                new Exception()
            );

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        /// <summary>
        /// Verifica il formato dei dettagli tecnici
        /// </summary>
        [Fact]
        public void TechnicalDetails_ContieneInformazioniComplete()
        {
            // Arrange
            var ex = new MemoryCacheException("Test", new Exception("Inner"));

            // Act
            var details = ex.TechnicalDetails;

            // Assert
            Assert.Contains("Cache Failure: Test", details);
            Assert.Contains("Inner Exception Type: System.Exception", details);
        }

        /// <summary>
        /// Verifica il comportamento con chiave non valida
        /// </summary>
        [Fact]
        public async Task SetAsync_ChiaveNonValida_GeneraErroreValidazione()
        {
            // Arrange
            _mockKeyValidator
                .Setup(v => v.Validate(It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _provider.SetAsync("chiave_non_valida!", "valore");

            // Assert
            Assert.Equal(CacheOperationStatus.ValidationError, result.Status);
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