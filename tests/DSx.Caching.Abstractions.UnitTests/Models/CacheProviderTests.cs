using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per l'interfaccia ICacheProvider
    /// </summary>
    public class CacheProviderTests
    {
        /// <summary>
        /// Verifica che GetAsync restituisca un valore esistente
        /// </summary>
        [Fact]
        public async Task GetAsync_ReturnsValue_WhenKeyExists()
        {
            // Arrange
            const string testKey = "test_key";
            const string expectedValue = "test_value";

            var mockProvider = new Mock<ICacheProvider>();
            mockProvider
                .Setup(x => x.GetAsync<string>(
                    testKey,
                    It.IsAny<CacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult<string>
                {
                    Status = CacheOperationStatus.Success,
                    Value = expectedValue
                });

            // Act
            var result = await mockProvider.Object.GetAsync<string>(testKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be(expectedValue);
        }
    }
}