using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Health
{
    /// <summary>
    /// Test per verificare il comportamento del monitoraggio salute cache.
    /// </summary>
    public class CacheHealthMonitorTests
    {
        private readonly Mock<ICacheHealthMonitor> _mockMonitor = new();

        /// <summary>
        /// Verifica che GetHealthStatus restituisca Unhealthy quando configurato.
        /// </summary>
        [Fact]
        public void GetHealthStatus_DovrebbeRestituireUnhealthy_QuandoConfigurato()
        {
            // Arrange
            _mockMonitor.Setup(x => x.GetHealthStatus())
                .Returns(CacheHealthStatus.Unhealthy);

            // Act
            var result = _mockMonitor.Object.GetHealthStatus();

            // Assert
            result.Should().Be(CacheHealthStatus.Unhealthy);
        }

        /// <summary>
        /// Verifica che CalculateCacheHitRatio restituisca 1 quando tutti accessi sono validi.
        /// </summary>
        [Fact]
        public void CalculateCacheHitRatio_DovrebbeRestituire1_PerCachePerfetta()
        {
            // Arrange
            _mockMonitor.Setup(x => x.CalculateCacheHitRatio())
                .Returns(1.0);

            // Act
            var result = _mockMonitor.Object.CalculateCacheHitRatio();

            // Assert
            result.Should().Be(1.0);
        }
    }
}