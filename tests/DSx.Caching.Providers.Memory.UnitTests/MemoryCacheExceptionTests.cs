using DSx.Caching.Abstractions.Models;
using DSx.Caching.Providers.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Test per MemoryCacheProvider
    /// </summary>
    public class MemoryCacheProviderTests : IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MemoryCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Inizializza il test
        /// </summary>
        public MemoryCacheProviderTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _provider = new MemoryCacheProvider(
                _mockCache.Object,
                new Mock<ILogger<MemoryCacheProvider>>().Object);
        }

        /// <summary>
        /// Verifica che ClearAllAsync svuoti la cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_ClearsCache()
        {
            // Arrange
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var provider = new MemoryCacheProvider(
                memoryCache,
                new Mock<ILogger<MemoryCacheProvider>>().Object);

            // Act
            var result = await provider.ClearAllAsync();

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            // Verifica indiretta che la cache sia stata svuotata
            Assert.Equal(0, memoryCache.Count);
        }

        /// <summary>
        /// Rilascia le risorse
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _provider.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}