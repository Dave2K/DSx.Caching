using DSx.Caching.Abstractions.Models;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per verificare il comportamento della classe CacheEntryOptions
    /// </summary>
    public class CacheEntryOptionsTests
    {
        /// <summary>
        /// Verifica che venga generata un'eccezione per valori di scadenza negativi
        /// </summary>
        [Fact]
        public void CacheEntryOptions_DovrebbeValidareValoriNegativi()
        {
            // Arrange
            var options = new CacheEntryOptions();

            // Act & Assert - AbsoluteExpiration negativo
            var exAbsolute = Assert.Throws<ArgumentException>(() =>
                options.AbsoluteExpiration = TimeSpan.FromTicks(-1));
            Assert.Contains("non può essere negativo", exAbsolute.Message);

            // Act & Assert - SlidingExpiration negativo
            var exSliding = Assert.Throws<ArgumentException>(() =>
                options.SlidingExpiration = TimeSpan.FromMinutes(-5));
            Assert.Contains("non può essere negativo", exSliding.Message);
        }

        /// <summary>
        /// Verifica l'assegnazione corretta dei valori di priorità
        /// </summary>
        [Fact]
        public void CacheEntryOptions_Priority_DovrebbeAccettareValoriValidi()
        {
            // Arrange
            var options = new CacheEntryOptions();

            // Act
            options.Priority = CacheEntryPriority.High;

            // Assert
            Assert.Equal(CacheEntryPriority.High, options.Priority);
        }
    }
}