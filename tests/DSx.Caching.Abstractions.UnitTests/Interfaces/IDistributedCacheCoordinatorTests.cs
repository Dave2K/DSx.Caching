// SOSTITUIRE TUTTO il contenuto del file
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
    /// Test per il coordinatore distribuito di cache
    /// </summary>
    public class DistributedCacheCoordinatorTests
    {
        private readonly Mock<IDistributedCacheCoordinator> _mockCoordinator = new();

        /// <summary>
        /// Verifica l'acquisizione del lock distribuito
        /// </summary>
        [Fact]
        public async Task AcquireLockAsync_ReturnsDisposableLock()
        {
            var disposableMock = new Mock<IDisposable>();
            _mockCoordinator
                .Setup(x => x.AcquireLockAsync(
                    "key",
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(disposableMock.Object);

            var lockObject = await _mockCoordinator.Object.AcquireLockAsync(
                "key",
                TimeSpan.FromSeconds(10));

            lockObject.Should().NotBeNull();
        }
    }
}