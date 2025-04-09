using DSx.Caching.Abstractions.Models;
using DSx.Caching.Providers.Memory;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="MemoryCacheProvider"/>
    /// </summary>
    public class MemoryCacheProviderTests : IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MemoryCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore per l'inizializzazione dei test
        /// </summary>
        public MemoryCacheProviderTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _provider = new MemoryCacheProvider(
                _mockCache.Object,
                new Mock<ILogger<MemoryCacheProvider>>().Object);
        }

        /// <summary>
        /// Verifica il corretto funzionamento dello svuotamento completo della cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_SvuotaCorrettamenteLaCache()
        {
            // Arrange
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var provider = new MemoryCacheProvider(
                memoryCache,
                new Mock<ILogger<MemoryCacheProvider>>().Object);

            // Act
            var result = await provider.ClearAllAsync();

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            memoryCache.Count.Should().Be(0, "La cache dovrebbe essere completamente svuotata");
        }

        /// <summary>
        /// Verifica la corretta gestione della disposizione delle risorse
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

        /// <summary>
        /// Verifica il comportamento del provider con cache null
        /// </summary>
        [Fact]
        public void Costruttore_ConCacheNull_GeneraEccezione()
        {
            // Arrange
            IMemoryCache cacheNull = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MemoryCacheProvider(
                    cacheNull,
                    new Mock<ILogger<MemoryCacheProvider>>().Object));
        }

        /// <summary>
        /// Verifica il comportamento del provider con logger null
        /// </summary>
        [Fact]
        public void Costruttore_ConLoggerNull_GeneraEccezione()
        {
            // Arrange
            ILogger<MemoryCacheProvider> loggerNull = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new MemoryCacheProvider(
                    new Mock<IMemoryCache>().Object,
                    loggerNull));
        }

        /// <summary>
        /// Verifica la rimozione corretta di una chiave esistente
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ChiaveEsistente_RimozioneCorretta()
        {
            // Arrange
            const string testKey = "chiave_test";
            _mockCache.Setup(c => c.CreateEntry(testKey)).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _provider.RemoveAsync(testKey);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            _mockCache.Verify(c => c.Remove(testKey), Times.Once);
        }
    }
}