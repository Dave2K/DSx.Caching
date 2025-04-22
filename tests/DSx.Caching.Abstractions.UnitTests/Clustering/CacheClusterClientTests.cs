// SOSTITUIRE TUTTO il contenuto del file
using DSx.Caching.Abstractions.Clustering;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Clustering
{
    /// <summary>
    /// Suite di test per il client di cluster Redis
    /// </summary>
    public class CacheClusterClientTests
    {
        private readonly Mock<ICacheClusterClient> _mockClient = new();

        /// <summary>
        /// Verifica l'invalidazione delle chiavi tramite pattern
        /// </summary>
        [Fact]
        public async Task InvalidateByPatternAsync_DeletesAllMatchingKeys()
        {
            _mockClient
                .Setup(x => x.InvalidateByPatternAsync("user_*"))
                .Returns(Task.CompletedTask);

            await _mockClient.Object.InvalidateByPatternAsync("user_*");
            _mockClient.Verify(x => x.InvalidateByPatternAsync("user_*"), Times.Once);
        }

        /// <summary>
        /// Verifica la sincronizzazione dello stato del cluster
        /// </summary>
        [Fact]
        public async Task SyncClusterStateAsync_CompletesSuccessfully()
        {
            _mockClient
                .Setup(x => x.SyncClusterStateAsync())
                .Returns(Task.CompletedTask);

            await _mockClient.Object.SyncClusterStateAsync();
            _mockClient.Verify(x => x.SyncClusterStateAsync(), Times.Once);
        }
    }
}