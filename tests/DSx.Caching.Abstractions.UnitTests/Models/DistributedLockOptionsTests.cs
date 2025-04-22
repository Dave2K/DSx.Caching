using DSx.Caching.Abstractions.Models;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per verificare i valori predefiniti delle opzioni di lock distribuito
    /// </summary>
    public class DistributedLockOptionsTests
    {
        /// <summary>
        /// Verifica che i valori di default siano impostati correttamente
        /// </summary>
        [Fact]
        public void DistributedLockOptions_DovrebbeAvereDefaultCorretti()
        {
            // Arrange
            var options = new DistributedLockOptions();

            // Assert
            Assert.Equal(30, options.Timeout.TotalSeconds);
            Assert.Equal(3, options.RetryAttempts);
            Assert.Equal(500, options.RetryInterval.TotalMilliseconds);
        }
    }
}