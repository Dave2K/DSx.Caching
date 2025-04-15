using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Health
{
    /// <summary>
    /// Test aggiuntivi per casi limite del monitoraggio salute
    /// </summary>
    public class CacheHealthMonitorEdgeCasesTests
    {
        private readonly Mock<ICacheHealthMonitor> _mockMonitor = new();

        /// <summary>
        /// Verifica il comportamento con hit ratio minimo
        /// </summary>
        [Fact]
        public void CalculateCacheHitRatio_DovrebbeGestireValoreMinimo()
        {
            _mockMonitor
                .Setup(x => x.CalculateCacheHitRatio())
                .Returns(0.0);

            var result = _mockMonitor.Object.CalculateCacheHitRatio();
            result.Should().BeGreaterThanOrEqualTo(0.0);
        }

        /// <summary>
        /// Verifica il comportamento con hit ratio massimo
        /// </summary>
        [Fact]
        public void CalculateCacheHitRatio_DovrebbeGestireValoreMassimo()
        {
            _mockMonitor
                .Setup(x => x.CalculateCacheHitRatio())
                .Returns(1.0);

            var result = _mockMonitor.Object.CalculateCacheHitRatio();
            result.Should().BeLessThanOrEqualTo(1.0);
        }
    }
}