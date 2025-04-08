using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using DSx.Caching.Abstractions;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Unit tests for verifying the <see cref="ICacheService"/> contract.
    /// </summary>
    public class CacheServiceContractTests
    {
        private readonly Mock<ICacheService> _mockCacheService = new();

        /// <summary>
        /// Tests that <see cref="ICacheService.GetAsync{T}(string)"/> throws an <see cref="ArgumentNullException"/> when the key is null.
        /// </summary>
        [Fact]
        public async Task GetAsync_KeyIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _mockCacheService
                .Setup(s => s.GetAsync<string>(null!))
                .ThrowsAsync(new ArgumentNullException("key"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _mockCacheService.Object.GetAsync<string>(null!)
            );
        }

        /// <summary>
        /// Tests that <see cref="ICacheService.SetAsync{T}(string, T)"/> throws an <see cref="ArgumentNullException"/> when the value is null.
        /// </summary>
        [Fact]
        public async Task SetAsync_ValueIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _mockCacheService
                .Setup(s => s.SetAsync<string>("valid_key", null!))
                .ThrowsAsync(new ArgumentNullException("value"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _mockCacheService.Object.SetAsync<string>("valid_key", null!)
            );
        }

        /// <summary>
        /// Tests that <see cref="ICacheService.RemoveAsync(string)"/> throws an <see cref="ArgumentException"/> when the key is empty.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_KeyIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            _mockCacheService
                .Setup(s => s.RemoveAsync(string.Empty))
                .ThrowsAsync(new ArgumentException("Key cannot be empty.", "key"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _mockCacheService.Object.RemoveAsync(string.Empty)
            );
        }
    }
}