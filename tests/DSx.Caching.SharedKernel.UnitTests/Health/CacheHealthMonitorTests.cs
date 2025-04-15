using DSx.Caching.Abstractions.Health;
using DSx.Caching.SharedKernel.Health;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Health
{
    /// <summary>
    /// Test per la classe CacheHealthMonitor
    /// </summary>
    public class CacheHealthMonitorTests
    {
        private readonly Mock<ICacheMetricsProvider> _mockMetrics = new();
        private readonly SharedKernel.Health.CacheHealthMonitor _monitor;

        /// <summary>
        /// Inizializza un nuovo test
        /// </summary>
        public CacheHealthMonitorTests()
        {
            _monitor = new SharedKernel.Health.CacheHealthMonitor(_mockMetrics.Object);
        }

        /// <summary>
        /// Verifica lo stato di salute in base al tasso di errori
        /// </summary>
        [Theory]
        [InlineData(0.009, CacheHealthStatus.Healthy)]
        [InlineData(0.05, CacheHealthStatus.Degraded)]
        [InlineData(0.11, CacheHealthStatus.Unhealthy)]
        public void GetHealthStatus_ShouldReturnCorrectStatus(double errorRate, CacheHealthStatus expected)
        {
            _mockMetrics.Setup(m => m.GetCurrentMetrics()).Returns(new CacheMetrics
            {
                ErrorRate = errorRate,
                TotalHits = 1000,
                TotalMisses = 200
            });

            var result = _monitor.GetHealthStatus();
            result.Should().Be(expected);
        }
    }
}