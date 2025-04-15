using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Models
{
    /// <summary>
    /// Test per la classe <see cref="CacheOperationResult"/> e <see cref="CacheOperationResult{T}"/>.
    /// </summary>
    public class CacheOperationResultTests
    {
        /// <summary>
        /// Verifica che <see cref="CacheOperationResult.IsSuccess"/> sia true 
        /// quando lo status è <see cref="CacheOperationStatus.Success"/>.
        /// </summary>
        [Fact]
        public void IsSuccess_ShouldBeTrue_WhenStatusIsSuccess()
        {
            // Arrange
            var result = new CacheOperationResult { Status = CacheOperationStatus.Success };

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        /// <summary>
        /// Verifica che <see cref="CacheOperationResult.IsSuccess"/> sia false 
        /// per stati diversi da <see cref="CacheOperationStatus.Success"/>.
        /// </summary>
        [Theory]
        [InlineData(CacheOperationStatus.NotFound)]
        [InlineData(CacheOperationStatus.ValidationError)]
        [InlineData(CacheOperationStatus.ConnectionError)]
        public void IsSuccess_ShouldBeFalse_WhenStatusIsNotSuccess(CacheOperationStatus status)
        {
            // Arrange
            var result = new CacheOperationResult { Status = status };

            // Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}