using DSx.Caching.SharedKernel.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Models
{
    /// <summary>
    /// Test per la classe <see cref="CacheEntryOptions"/>.
    /// </summary>
    public class CacheEntryOptionsTests
    {
        /// <summary>
        /// Verifica che il costruttore imposti correttamente <see cref="CacheEntryOptions.SlidingExpiration"/>.
        /// </summary>
        [Fact]
        public void Constructor_SetsSlidingExpiration()
        {
            // Arrange
            var slidingExpiration = TimeSpan.FromMinutes(15);

            // Act
            var options = new CacheEntryOptions(slidingExpiration: slidingExpiration);

            // Assert
            options.SlidingExpiration.Should().Be(slidingExpiration);
        }

        /// <summary>
        /// Verifica che il costruttore imposti correttamente <see cref="CacheEntryOptions.AbsoluteExpiration"/>.
        /// </summary>
        [Fact]
        public void Constructor_SetsAbsoluteExpiration()
        {
            // Arrange
            var absoluteExpiration = TimeSpan.FromHours(1);

            // Act
            var options = new CacheEntryOptions(absoluteExpiration: absoluteExpiration);

            // Assert
            options.AbsoluteExpiration.Should().Be(absoluteExpiration);
        }
    }
}