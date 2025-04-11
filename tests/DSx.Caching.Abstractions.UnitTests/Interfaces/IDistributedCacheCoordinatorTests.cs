using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per <see cref="IDistributedCacheCoordinator"/>
    /// </summary>
    public class DistributedCacheCoordinatorTests
    {
        private readonly Mock<IDistributedCacheCoordinator> _mockCoordinator = new();

        /// <summary>
        /// Verifica che AcquireLockAsync restituisca un lock valido
        /// </summary>
        [Fact]
        public async Task AcquireLockAsync_ReturnsDisposableLock()
        {
            // Arrange
            var disposableMock = new Mock<IDisposable>();
            _mockCoordinator
                .Setup(x => x.AcquireLockAsync("key", It.IsAny<TimeSpan>()))
                .ReturnsAsync(disposableMock.Object);

            // Act
            var lockObject = await _mockCoordinator.Object.AcquireLockAsync("key", TimeSpan.FromSeconds(10));

            // Assert
            lockObject.Should().NotBeNull();
        }

        /// <summary>
        /// Verifica che InvalidateAcrossNodesAsync completi l'operazione
        /// </summary>
        [Fact]
        public async Task InvalidateAcrossNodesAsync_CompletesSuccessfully()
        {
            // Arrange
            _mockCoordinator
                .Setup(x => x.InvalidateAcrossNodesAsync("key"))
                .Returns(Task.CompletedTask);

            // Act & Assert (no exceptions)
            await _mockCoordinator.Object.InvalidateAcrossNodesAsync("key");
        }
    }
}