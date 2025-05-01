using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Clustering
{
    /// <summary>
    /// Suite di test per il client di cluster Redis.
    /// </summary>
    public class CacheClusterClientTests
    {
        private readonly Mock<ICacheClusterClient> _mockClient = new();

        /// <summary>
        /// Verifica l'invalidazione delle chiavi tramite pattern.
        /// </summary>
        [Fact]
        public async Task InvalidateByPatternAsync_DeveEseguireOperazioneConPatternCorretto()
        {
            // Arrange
            const string pattern = "test_*";
            _mockClient.Setup(x => x.InvalidateByPatternAsync(pattern))
                .Returns(Task.CompletedTask);

            // Act
            await _mockClient.Object.InvalidateByPatternAsync(pattern);

            // Assert
            _mockClient.Verify(x => x.InvalidateByPatternAsync(pattern), Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo BroadcastInvalidationAsync sia chiamato correttamente.
        /// </summary>
        [Fact]
        public async Task BroadcastInvalidationAsync_DeveChiamareIlMetodoConLaChiave()
        {
            // Arrange
            const string key = "chiave_123";
            _mockClient.Setup(x => x.BroadcastInvalidationAsync(key))
                .Returns(Task.CompletedTask);

            // Act
            await _mockClient.Object.BroadcastInvalidationAsync(key);

            // Assert
            _mockClient.Verify(x => x.BroadcastInvalidationAsync(key), Times.Once);
        }
    }
}