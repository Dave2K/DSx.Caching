using DSx.Caching.Abstractions.Events;
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
    /// Classe di test per BaseCacheProvider
    /// </summary>
    public class BaseCacheProviderTests
    {
        /// <summary>
        /// Implementazione concreta per il testing
        /// </summary>
        private class TestCacheProvider : BaseCacheProvider
        {
            /// <summary>
            /// Inizializza una nuova istanza del provider di test
            /// </summary>
            /// <param name="logger">Logger per la gestione dei log</param>
            public TestCacheProvider(ILogger logger) : base(logger) { }

            /// <summary>
            /// Verifica lo stato di disposizione dell'oggetto
            /// </summary>
            public void VerificaDisposizione() => CheckDisposed();

            /// <summary>
            /// Implementazione del metodo GetAsync
            /// </summary>
            public override Task<CacheOperationResult<T>> GetAsync<T>(
                string key,
                CacheEntryOptions? options = null,
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del metodo SetAsync
            /// </summary>
            public override Task<CacheOperationResult> SetAsync<T>(
                string key,
                T value,
                CacheEntryOptions? options = null,
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del metodo RemoveAsync
            /// </summary>
            public override Task<CacheOperationResult> RemoveAsync(
                string key,
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del metodo GetDescriptorAsync
            /// </summary>
            public override Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
                string key,
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del metodo ClearAllAsync
            /// </summary>
            public override Task<CacheOperationResult> ClearAllAsync(
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del metodo ExistsAsync
            /// </summary>
            public override Task<CacheOperationResult<bool>> ExistsAsync(
                string key,
                CancellationToken cancellationToken = default)
                => throw new NotImplementedException();

            /// <summary>
            /// Implementazione del Dispose sincrono
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
            }

            /// <summary>
            /// Implementazione dell'async dispose pattern
            /// </summary>
            protected override async ValueTask DisposeAsyncCore()
            {
                await base.DisposeAsyncCore().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifica la corretta gestione della disposizione
        /// </summary>
        [Fact]
        public void CheckDisposed_DovrebbeSollevareEccezione_QuandoDisposed()
        {
            var loggerMock = new Mock<ILogger>();
            var provider = new TestCacheProvider(loggerMock.Object);
            provider.Dispose();
            Assert.Throws<ObjectDisposedException>(() => provider.VerificaDisposizione());
        }

        /// <summary>
        /// Verifica il corretto funzionamento del Dispose asincrono
        /// </summary>
        [Fact]
        public async Task DisposeAsync_DovrebbeSegnareComeDisposed()
        {
            var loggerMock = new Mock<ILogger>();
            var provider = new TestCacheProvider(loggerMock.Object);
            await provider.DisposeAsync();
            Assert.Throws<ObjectDisposedException>(() => provider.VerificaDisposizione());
        }
    }
}
