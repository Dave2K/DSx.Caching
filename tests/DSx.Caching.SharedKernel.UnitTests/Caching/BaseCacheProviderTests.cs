using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Caching;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Caching
{
    /// <summary>
    /// Test suite per la verifica del comportamento del BaseCacheProvider
    /// </summary>
    public class BaseCacheProviderTests
    {
        /// <summary>
        /// Implementazione concreta per testing
        /// </summary>
        private class TestProvider(ILogger logger) : BaseCacheProvider(logger)
        {
            /// <inheritdoc/>
            public override Task<CacheOperationResult> ClearAllAsync(CancellationToken ct = default)
            {
                CheckDisposed();
                return Task.FromResult(new CacheOperationResult());
            }

            /// <inheritdoc/>
            public override Task<CacheOperationResult> ExistsAsync(
                string key,
                CacheEntryOptions? options = null,
                CancellationToken ct = default)
            {
                CheckDisposed();
                return Task.FromResult(new CacheOperationResult());
            }

            /// <inheritdoc/>
            public override Task<CacheOperationResult<T>> GetAsync<T>(
                string key,
                CacheEntryOptions? options = null,
                CancellationToken ct = default)
            {
                CheckDisposed();
                return Task.FromResult(new CacheOperationResult<T>());
            }

            /// <inheritdoc/>
            public override Task<CacheOperationResult> RemoveAsync(string key, CancellationToken ct = default)
            {
                CheckDisposed();
                return Task.FromResult(new CacheOperationResult());
            }

            /// <inheritdoc/>
            public override Task<CacheOperationResult> SetAsync<T>(
                string key,
                T value,
                CacheEntryOptions? options = null,
                CancellationToken ct = default)
            {
                CheckDisposed();
                return Task.FromResult(new CacheOperationResult());
            }
        }

        /// <summary>
        /// Verifica che dopo Dispose non sia possibile eseguire operazioni
        /// </summary>
        [Fact]
        public async Task Dispose_ShouldPreventOperations_Async()
        {
            // Arrange
            var provider = new TestProvider(Mock.Of<ILogger>());
            provider.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await provider.GetAsync<string>("test")
            );
        }
    }
}