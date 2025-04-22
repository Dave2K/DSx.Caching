using DSx.Caching.Abstractions.Models;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per verificare i valori dell'enumerazione ClusterStatus
    /// </summary>
    public class ClusterStatusTests
    {
        /// <summary>
        /// Verifica la corretta mappatura dei valori numerici
        /// </summary>
        [Fact]
        public void ClusterStatus_DovrebbeAvereValoriCorretti()
        {
            // Assert
            Assert.Equal(0, (int)ClusterStatus.Healthy);
            Assert.Equal(1, (int)ClusterStatus.Degraded);
            Assert.Equal(2, (int)ClusterStatus.Unhealthy);
        }
    }
}