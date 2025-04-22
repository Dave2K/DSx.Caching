using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per le opzioni di configurazione della cache.
    /// </summary>
    public class CacheEntryOptionsTests
    {
        /// <summary>
        /// Verifica che AbsoluteExpiration accetti solo valori positivi.
        /// </summary>
        [Theory]
        [InlineData(0)]  // TimeSpan.Zero
        [InlineData(-1)] // Valore negativo
        public void AbsoluteExpiration_DovrebbeRifiutareValoriNonPositivi(int seconds)
        {
            // Arrange
            var options = new CacheEntryOptions();
            var invalidValue = TimeSpan.FromSeconds(seconds);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => options.AbsoluteExpiration = invalidValue);
        }

        /// <summary>
        /// Verifica che SlidingExpiration accetti solo valori positivi.
        /// </summary>
        [Fact]
        public void SlidingExpiration_DovrebbeRifiutareTimeSpanZero()
        {
            // Arrange
            var options = new CacheEntryOptions();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => options.SlidingExpiration = TimeSpan.Zero);
        }

        /// <summary>
        /// Verifica che Priority sia Normal di default.
        /// </summary>
        [Fact]
        public void Priority_DovrebbeAvereValoreDefaultNormal()
        {
            // Arrange & Act
            var options = new CacheEntryOptions();

            // Assert
            options.Priority.Should().Be(CacheEntryPriority.Normal);
        }
    }
}