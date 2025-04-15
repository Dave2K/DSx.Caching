using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Health
{
    /// <summary>
    /// Test per l'enum CacheHealthStatus
    /// </summary>
    public class CacheHealthStatusTests
    {
        /// <summary>
        /// Verifica che i valori dell'enum siano corretti
        /// </summary>
        [Theory]
        [InlineData(0, CacheHealthStatus.Healthy)]
        [InlineData(1, CacheHealthStatus.Degraded)]
        [InlineData(2, CacheHealthStatus.Unhealthy)]
        [InlineData(3, CacheHealthStatus.Unknown)]
        public void EnumValues_DovrebberoAvereOrdineCorretto(
            int expectedValue,
            CacheHealthStatus status)
        {
            ((int)status).Should().Be(expectedValue);
        }
    }
}