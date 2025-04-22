using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per verificare il comportamento del provider di cache
    /// </summary>
    public class CacheProviderTests
    {
        private readonly Mock<ICacheProvider> _mockProvider = new();

        /// <summary>
        /// Verifica che GetAsync restituisca il valore corretto quando la chiave esiste
        /// </summary>
        [Fact]
        public async Task GetAsync_ReturnsValue_WhenKeyExists()
        {
            // Arrange
            const string testKey = "valid_key";
            const string expectedValue = "test_value";

            _mockProvider
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
            var result = await _mockProvider.Object.GetAsync<string>(testKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be(expectedValue);
        }

        /// <summary>
        /// Verifica che GetAsync restituisca errore di connessione in caso di fallimento
        /// </summary>
        [Fact]
        public async Task GetAsync_ReturnsConnectionError_WhenProviderFails()
        {
            // Arrange
            const string testKey = "invalid_key";

            _mockProvider
                .Setup(x => x.GetAsync<string>(
                    testKey,
                    It.IsAny<CacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult<string>
                {
                    Status = CacheOperationStatus.ConnectionError,
                    Details = "Simulated failure"
                });

            // Act
            var result = await _mockProvider.Object.GetAsync<string>(testKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ConnectionError);
            result.Details.Should().NotBeNullOrEmpty();
        }
    }
}