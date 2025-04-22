using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per verificare il comportamento del provider di cache.
    /// </summary>
    public class CacheProviderTests
    {
        private readonly Mock<ICacheProvider> _mockProvider = new();

        /// <summary>
        /// Verifica che GetAsync restituisca il valore corretto quando la chiave esiste.
        /// </summary>
        [Fact]
        public async Task GetAsync_DovrebbeRestituireValore_SeChiaveEsiste()
        {
            // Arrange
            const string key = "chiave_valida";
            const string value = "valore_test";
            _mockProvider.Setup(x => x.GetAsync<string>(key, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult<string> { Status = CacheOperationStatus.Success, Value = value });

            // Act
            var result = await _mockProvider.Object.GetAsync<string>(key);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be(value);
        }

        /// <summary>
        /// Verifica che RemoveAsync completi l'operazione con successo.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_DovrebbeCompletareConSuccesso()
        {
            // Arrange
            const string key = "chiave_da_rimuovere";
            _mockProvider.Setup(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult { Status = CacheOperationStatus.Success });

            // Act
            var result = await _mockProvider.Object.RemoveAsync(key);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
        }
    }
}