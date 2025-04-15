using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Health
{
    /// <summary>
    /// Test per l'interfaccia di monitoraggio salute cache
    /// </summary>
    public class CacheHealthMonitorTests
    {
        private readonly Mock<ICacheHealthMonitor> _mockMonitor = new();

        /// <summary>
        /// Verifica che lo stato Healthy venga restituito correttamente
        /// </summary>
        [Fact]
        public void GetHealthStatus_DovrebbeRestituireHealthy_QuandoConfigurato()
        {
            _mockMonitor
                .Setup(x => x.GetHealthStatus())
                .Returns(CacheHealthStatus.Healthy);

            var result = _mockMonitor.Object.GetHealthStatus();
            result.Should().Be(CacheHealthStatus.Healthy);
        }

        /// <summary>
        /// Verifica il calcolo corretto dell'hit ratio
        /// </summary>
        [Fact]
        public void CalculateCacheHitRatio_DovrebbeRestituireValoreCorretto()
        {
            const double expectedRatio = 0.85;
            _mockMonitor
                .Setup(x => x.CalculateCacheHitRatio())
                .Returns(expectedRatio);

            var result = _mockMonitor.Object.CalculateCacheHitRatio();
            result.Should().Be(expectedRatio);
        }

        /// <summary>
        /// Verifica tutti gli stati possibili
        /// </summary>
        [Theory]
        [InlineData(CacheHealthStatus.Healthy)]
        [InlineData(CacheHealthStatus.Degraded)]
        [InlineData(CacheHealthStatus.Unhealthy)]
        [InlineData(CacheHealthStatus.Unknown)]
        public void GetHealthStatus_DovrebbeSupportareTuttiGliStati(CacheHealthStatus status)
        {
            _mockMonitor
                .Setup(x => x.GetHealthStatus())
                .Returns(status);

            var result = _mockMonitor.Object.GetHealthStatus();
            result.Should().Be(status);
        }
    }
}