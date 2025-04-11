using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per <see cref="CacheEntryOptions"/>
    /// </summary>
    public class CacheEntryOptionsTests
    {
        /// <summary>
        /// Verifica che AbsoluteExpiration sia impostabile correttamente
        /// </summary>
        [Fact]
        public void AbsoluteExpiration_SetsCorrectValue()
        {
            // Arrange
            var options = new CacheEntryOptions();

            // Act
            options.AbsoluteExpiration = TimeSpan.FromMinutes(30);

            // Assert
            options.AbsoluteExpiration.Should().Be(TimeSpan.FromMinutes(30));
        }
    }
}