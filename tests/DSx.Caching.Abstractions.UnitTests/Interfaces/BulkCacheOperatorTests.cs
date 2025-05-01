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
    /// Test per verificare le operazioni bulk su cache.
    /// </summary>
    public class BulkCacheOperatorTests
    {
        private readonly Mock<IBulkCacheOperator> _mockBulkOperator = new();
        private readonly Dictionary<string, string> _testItems = new()
        {
            {"chiave1", "valore1"},
            {"chiave2", "valore2"},
            {"chiave3", "valore3"}
        };

        /// <summary>
        /// Verifica che BulkSetAsync inserisca correttamente più elementi.
        /// </summary>
        [Fact]
        public async Task BulkSetAsync_DovrebbeInserireMultipliElementi()
        {
            // Arrange
            _mockBulkOperator
                .Setup(x => x.BulkSetAsync<string>(
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CacheEntryOptions?>()))
                .Returns(Task.FromResult(new CacheOperationResult(CacheOperationStatus.Success)));

            // Act
            CacheOperationResult result = await _mockBulkOperator.Object.BulkSetAsync(_testItems);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            _mockBulkOperator.Verify(x =>
                x.BulkSetAsync<string>(_testItems, It.IsAny<CacheEntryOptions?>()), Times.Once);
        }

        /// <summary>
        /// Verifica che BulkGetAsync recuperi elementi esistenti.
        /// </summary>
        [Fact]
        public async Task BulkGetAsync_DovrebbeRecuperareElementiEsistenti()
        {
            // Arrange
            var keys = new List<string> { "chiave1", "chiave2" };
            _mockBulkOperator
                .Setup(x => x.BulkGetAsync<string>(keys))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    {"chiave1", "valore1"},
                    {"chiave2", "valore2"}
                });

            // Act
            IDictionary<string, string> result = await _mockBulkOperator.Object.BulkGetAsync<string>(keys);

            // Assert
            result.Should().HaveCount(2);
            result["chiave1"].Should().Be("valore1");
        }

        /// <summary>
        /// Verifica il comportamento con collezione vuota.
        /// </summary>
        [Fact]
        public async Task BulkSetAsync_DovrebbeGestireCollezioneVuota()
        {
            // Arrange
            var emptyItems = new Dictionary<string, string>();
            _mockBulkOperator
                .Setup(x => x.BulkSetAsync<string>(
                    emptyItems,
                    It.IsAny<CacheEntryOptions?>()))
                .Returns(Task.FromResult(new CacheOperationResult(CacheOperationStatus.ValidationError)));

            // Act
            CacheOperationResult result = await _mockBulkOperator.Object.BulkSetAsync(emptyItems);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ValidationError);
        }

        /// <summary>
        /// Verifica il fallimento con chiavi non valide.
        /// </summary>
        [Fact]
        public async Task BulkSetAsync_DovrebbeFallirePerChiaviNonValide()
        {
            // Arrange
            var invalidItems = new Dictionary<string, string> { { "chiave_invalida!", "valore" } };
            _mockBulkOperator
                .Setup(x => x.BulkSetAsync<string>(
                    invalidItems,
                    It.IsAny<CacheEntryOptions?>()))
                .Returns(Task.FromResult(
                    new CacheOperationResult(
                        CacheOperationStatus.ValidationError,
                        "Formato chiave non valido")
                ));

            // Act
            CacheOperationResult result = await _mockBulkOperator.Object.BulkSetAsync(invalidItems);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ValidationError);
            result.Details.Should().Contain("Formato chiave");
        }
    }
}
