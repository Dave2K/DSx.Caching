using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per la classe <see cref="CacheOperationResult"/>
    /// </summary>
    public class CacheOperationResultTests
    {
        /// <summary>
        /// Verifica che IsSuccess sia true quando lo status è Success
        /// </summary>
        [Fact]
        public void IsSuccess_ShouldBeTrue_WhenStatusIsSuccess()
        {
            // Arrange
            var result = new CacheOperationResult { Status = CacheOperationStatus.Success };

            // Act & Assert
            result.IsSuccess.Should().BeTrue();
        }

        /// <summary>
        /// Verifica che IsSuccess sia false per stati non Success
        /// </summary>
        [Theory]
        [InlineData(CacheOperationStatus.NotFound)]
        [InlineData(CacheOperationStatus.ValidationError)]
        [InlineData(CacheOperationStatus.ConnectionError)]
        public void IsSuccess_ShouldBeFalse_WhenStatusIsNotSuccess(CacheOperationStatus status)
        {
            // Arrange
            var result = new CacheOperationResult { Status = status };

            // Act & Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}