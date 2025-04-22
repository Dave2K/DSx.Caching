using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per verificare il comportamento del coordinatore distribuito.
    /// </summary>
    public class DistributedCacheCoordinatorTests
    {
        private readonly Mock<IDistributedCacheCoordinator> _mockCoordinator = new();

        /// <summary>
        /// Verifica l'acquisizione corretta del lock.
        /// </summary>
        [Fact]
        public async Task AcquireLockAsync_DovrebbeRestituireDisposable()
        {
            // Arrange
            var mockLock = new Mock<IDisposable>();
            _mockCoordinator
                .Setup(x => x.AcquireLockAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockLock.Object);

            // Act
            var result = await _mockCoordinator.Object.AcquireLockAsync(
                "test_key",
                TimeSpan.FromSeconds(1),
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Verifica che il metodo AcquireLockAsync rispetti la cancellazione del token.
        /// </summary>
        [Fact]
        public async Task AcquireLockAsync_DovrebbeFallire_SeCancellato()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockCoordinator
                .Setup(x => x.AcquireLockAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _mockCoordinator.Object.AcquireLockAsync(
                    "test_key",
                    TimeSpan.FromSeconds(1),
                    cts.Token));
        }
    }
}