using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Health
{
    /// <summary>
    /// Test per verificare il comportamento del provider di metriche.
    /// </summary>
    public class CacheMetricsProviderTests
    {
        private readonly Mock<ICacheMetricsProvider> _mockProvider = new();

        /// <summary>
        /// Verifica che GetCurrentMetrics restituisca valori corretti.
        /// </summary>
        [Fact]
        public void GetCurrentMetrics_DovrebbeRestituireMetricheCorrette()
        {
            // Arrange
            var expectedMetrics = new CacheMetrics
            {
                ErrorRate = 0.1,
                TotalHits = 90,
                TotalMisses = 10
            };

            _mockProvider.Setup(x => x.GetCurrentMetrics())
                .Returns(expectedMetrics);

            // Act
            var metrics = _mockProvider.Object.GetCurrentMetrics();

            // Assert
            metrics.Should().BeEquivalentTo(expectedMetrics);
        }

        /// <summary>
        /// Verifica il calcolo dell'hit ratio con valori estremi.
        /// </summary>
        [Fact]
        public void CalculateHitRatio_DovrebbeGestireDivisionePerZero()
        {
            // Arrange
            var metrics = new CacheMetrics
            {
                TotalHits = 0,
                TotalMisses = 0
            };

            // Act
            var ratio = metrics.CalculateHitRatio();

            // Assert
            ratio.Should().Be(0);
        }
    }
}