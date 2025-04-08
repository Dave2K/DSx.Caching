namespace DSx.Caching.UnitTests.Providers.Memory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DSx.Caching.Abstractions.Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class MemoryCacheProviderTests
    {
        private readonly Mock<IMemoryCache> _cacheMock = new();
        private readonly ILogger<MemoryCacheProvider> _logger = 
            Mock.Of<ILogger<MemoryCacheProvider>>();
        
        [Fact]
        public async Task SetAndGet_ValidKey_ReturnsValue()
        {
            // Arrange
            const string key = "test";
            const int value = 42;
            var provider = CreateProvider();
            
            _cacheMock
                .Setup(c => c.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            // Act
            await provider.SetAsync(key, value, TimeSpan.FromMinutes(1));
            var result = await provider.GetAsync<int>(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task GetAsync_InvalidKey_ThrowsArgumentException(string invalidKey)
        {
            // Arrange
            var provider = CreateProvider();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => provider.GetAsync<object>(invalidKey));
        }

        [Fact]
        public async Task ClearAllAsync_WhenCalled_CompactsCache()
        {
            // Arrange
            var memCache = new MemoryCache(new MemoryCacheOptions());
            var provider = new MemoryCacheProvider(memCache, _logger);

            // Act
            var result = await provider.ClearAllAsync();

            // Assert
            Assert.True(result);
            Assert.Equal(0, memCache.Count);
        }

        private MemoryCacheProvider CreateProvider() => 
            new(_cacheMock.Object, _logger);
    }
}