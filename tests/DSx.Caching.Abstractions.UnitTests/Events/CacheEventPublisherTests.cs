using DSx.Caching.Abstractions.Events;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Events
{
    /// <summary>
    /// Test per verificare il comportamento del pubblicatore di eventi della cache.
    /// </summary>
    public class CacheEventPublisherTests
    {
        private readonly Mock<ILogger<CacheEventPublisher>> _mockLogger;
        private readonly CacheEventPublisher _publisher;

        /// <summary>
        /// Inizializza una nuova istanza della classe di test.
        /// Configura il logger mock e il pubblicatore di eventi.
        /// </summary>
        public CacheEventPublisherTests()
        {
            _mockLogger = new Mock<ILogger<CacheEventPublisher>>();
            _publisher = new CacheEventPublisher(_mockLogger.Object);
        }

        /// <summary>
        /// Verifica che BeforeOperation venga generato correttamente.
        /// </summary>
        [Fact]
        public void NotifyBeforeOperation_DovrebbeGenerareEvento()
        {
            // Arrange
            var eventRaised = false;
            _publisher.BeforeOperation += (sender, args) => eventRaised = true;

            // Act
            _publisher.NotifyBeforeOperation("test_key", CacheOperationType.Get);

            // Assert
            Assert.True(eventRaised);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifica il logging degli eventi speciali.
        /// </summary>
        [Fact]
        public void NotifySpecialEvent_DovrebbeLoggareCorrettamente()
        {
            // Arrange
            var testException = new Exception("Test error");

            // Act
            _publisher.NotifySpecialEvent(CacheEvent.SerializationError, "Test message", testException);

            // Assert
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                CacheEvent.SerializationError,
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}