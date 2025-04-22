// SOSTITUIRE TUTTO il contenuto del file
using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per il servizio di cache
    /// </summary>
    public class CacheServiceTests
    {
        private readonly Mock<ICacheService> _mockService = new();

        /// <summary>
        /// Verifica la rimozione asincrona completata
        /// </summary>
        [Fact]
        public async Task RemoveAsync_CompletesSuccessfully()
        {
            _mockService
                .Setup(x => x.RemoveAsync("key", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _mockService.Object.RemoveAsync("key");
        }

        /// <summary>
        /// Verifica la gestione di valori nulli nell'inserimento
        /// </summary>
        [Fact]
        public async Task SetAsync_HandlesNullValues()
        {
            _mockService
                .Setup(x => x.SetAsync<string>(
                    "key",
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _mockService.Object.SetAsync("key", (string?)null);
        }
    }
}