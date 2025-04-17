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
    /// Test unitari per la classe <see cref="MemoryCacheProvider"/>
    /// </summary>
    public class MemoryCacheProviderTests
    {
        /// <summary>
        /// Verifica che le operazioni vengano correttamente annullate
        /// quando richiesto dal cancellation token
        /// </summary>
        [Fact]
        public async Task Operations_ShouldCancel_WhenTokenRequested()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<MemoryCacheProvider>>();
            var keyValidator = new Mock<ICacheKeyValidator>().Object;

            var provider = new MemoryCacheProvider(cache, loggerMock.Object, keyValidator);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await provider.SetAsync("key", "value", cancellationToken: cts.Token);

            // Assert
            Assert.Equal(CacheOperationStatus.OperationCancelled, result.Status);

            // Verifica corretta usando il metodo Log effettivo
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Errore durante il salvataggio chiave")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        /// <summary>
        /// Verifica il corretto salvataggio di una chiave valida
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldHandleValidKeys()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<MemoryCacheProvider>>();
            var keyValidator = new CacheKeyValidator();

            var provider = new MemoryCacheProvider(cache, loggerMock.Object, keyValidator);

            // Act
            var result = await provider.SetAsync("valid_key", "value");

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.True(cache.TryGetValue("valid_key", out string? _));
        }
    }
}