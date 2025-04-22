// File: DSx.Caching.Providers.Memory.UnitTests/MemoryCacheProviderTests.cs
using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Contiene i test per il provider di cache in memoria
    /// </summary>
    public class MemoryCacheProviderTests
    {
        /// <summary>
        /// Verifica che le operazioni vengano annullate correttamente
        /// </summary>
        [Fact]
        public async Task Operations_ShouldCancel_WhenTokenRequested()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<MemoryCacheProvider>>();
            var keyValidator = new Mock<ICacheKeyValidator>();
            keyValidator.Setup(v => v.Validate(It.IsAny<string>())).Verifiable();

            var provider = new MemoryCacheProvider(cache, loggerMock.Object, keyValidator.Object);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await provider.SetAsync("key", "value", cancellationToken: cts.Token);

            // Assert
            Assert.Equal(CacheOperationStatus.OperationCancelled, result.Status);
            keyValidator.Verify(v => v.Validate("key"), Times.Never);
        }

        /// <summary>
        /// Verifica il salvataggio corretto delle chiavi valide
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldHandleValidKeys()
        {
            // Configura
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<MemoryCacheProvider>>();
            var keyValidator = new CacheKeyValidator();

            var provider = new MemoryCacheProvider(cache, loggerMock.Object, keyValidator);

            // Esegui
            var result = await provider.SetAsync("valid_key", "value");

            // Verifica
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.True(cache.TryGetValue("valid_key", out _));
        }
    }
}