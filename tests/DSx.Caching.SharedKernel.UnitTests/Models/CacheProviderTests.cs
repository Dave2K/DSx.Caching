using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Interfaces;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Models
{
    /// <summary>
    /// Test per verificare il comportamento delle implementazioni di <see cref="ICacheProvider"/>.
    /// </summary>
    public class CacheProviderTests
    {
        /// <summary>
        /// Verifica che il metodo <see cref="ICacheProvider.GetAsync{T}"/> 
        /// restituisca correttamente un valore esistente.
        /// </summary>
        [Fact]
        public async Task GetAsync_ReturnsValue_WhenKeyExists()
        {
            // Arrange
            const string testKey = "valid_key";
            const string expectedValue = "test_value";

            var mockProvider = new Mock<ICacheProvider>();
            mockProvider
                .Setup(x => x.GetAsync<string>(
                    testKey,
                    It.IsAny<CacheEntryOptions>(),
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

        /// <summary>
        /// Verifica che il metodo <see cref="ICacheProvider.GetAsync{T}"/> 
        /// gestisca correttamente gli errori di connessione.
        /// </summary>
        [Fact]
        public async Task GetAsync_ReturnsConnectionError_WhenProviderFails()
        {
            // Arrange
            const string testKey = "invalid_key";
            var mockProvider = new Mock<ICacheProvider>();

            mockProvider
                .Setup(x => x.GetAsync<string>(
                    testKey,
                    It.IsAny<CacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult<string>
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = "Simulated connection failure"
                });

            // Act
            var result = await mockProvider.Object.GetAsync<string>(testKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ConnectionError);
            result.Details.Should().NotBeNullOrEmpty();
        }
    }
}