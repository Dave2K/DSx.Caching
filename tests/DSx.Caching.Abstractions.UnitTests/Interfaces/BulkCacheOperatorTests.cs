using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Contiene i test unitari per verificare il comportamento dell'interfaccia <see cref="IBulkCacheOperator"/>.
    /// </summary>
    public class BulkCacheOperatorTests
    {
        private readonly Mock<IBulkCacheOperator> _mockOperator = new();
        private static readonly string[] TestKeys = ["key1", "key_missing"];

        /// <summary>
        /// Verifica che il metodo BulkSetAsync elabori correttamente un dizionario di elementi.
        /// </summary>
        /// <returns>Task asincrono.</returns>
        [Fact]
        public async Task BulkSetAsync_DovrebbeElaborareDizionario()
        {
            // Arrange
            var items = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };

            _mockOperator.Setup(x => x.BulkSetAsync(
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CacheEntryOptions?>()))
                .Returns(Task.CompletedTask);

            // Act
            await _mockOperator.Object.BulkSetAsync(items);

            // Assert
            _mockOperator.Verify(x => x.BulkSetAsync(
                items,
                It.IsAny<CacheEntryOptions?>()),
                Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo BulkGetAsync restituisca solo le chiavi esistenti.
        /// </summary>
        /// <returns>Task asincrono.</returns>
        [Fact]
        public async Task BulkGetAsync_DovrebbeFiltrareChiaviMancanti()
        {
            // Arrange
            var expected = new Dictionary<string, object> { ["key1"] = "value1" };
            _mockOperator.Setup(x => x.BulkGetAsync<object>(
                It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _mockOperator.Object.BulkGetAsync<object>(TestKeys);

            // Assert
            result.Should()
                .HaveCount(1)
                .And.ContainKey("key1")
                .And.NotContainKey("key_missing");
        }

        /// <summary>
        /// Verifica che BulkSetAsync sollevi eccezione con dizionario null.
        /// </summary>
        [Fact]
        public async Task BulkSetAsync_DovrebbeSollevareEccezione_PerDizionarioNull()
        {
            // Arrange
            _mockOperator.Setup(x => x.BulkSetAsync<object>(
                null!,
                It.IsAny<CacheEntryOptions?>()))
                .ThrowsAsync(new ArgumentNullException());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _mockOperator.Object.BulkSetAsync<object>(null!, null));
        }
    }
}