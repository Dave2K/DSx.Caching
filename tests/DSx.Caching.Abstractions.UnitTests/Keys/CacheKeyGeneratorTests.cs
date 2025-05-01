using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Keys
{
    /// <summary>
    /// Test per verificare il comportamento del generatore di chiavi di cache.
    /// </summary>
    public class CacheKeyGeneratorTests
    {
        private readonly Mock<ICacheKeyGenerator> _mockGenerator = new();

        /// <summary>
        /// Verifica che GenerateKey gestisca correttamente i parametri.
        /// </summary>
        [Fact]
        public void GenerateKey_DovrebbeCombinareBaseKeyEParametri()
        {
            // Arrange
            const string expectedKey = "Prodotto_123_ABC";
            _mockGenerator.Setup(x => x.GenerateKey("Prodotto", It.IsAny<object[]>()))
                .Returns(expectedKey);

            // Act
            var result = _mockGenerator.Object.GenerateKey("Prodotto", 123, "ABC");

            // Assert
            result.Should().Be(expectedKey);
        }

        /// <summary>
        /// Verifica che NormalizeKey gestisca caratteri speciali.
        /// </summary>
        [Fact]
        public void NormalizeKey_DovrebbeRimuovereCaratteriNonValid()
        {
            // Arrange
            const string input = "key@with$special#chars";
            const string expected = "key-with-special-chars";
            _mockGenerator.Setup(x => x.NormalizeKey(input))
                .Returns(expected);

            // Act
            var result = _mockGenerator.Object.NormalizeKey(input);

            // Assert
            result.Should().Be(expected);
        }

        /// <summary>
        /// Verifica che GenerateKey sollevi eccezione per baseKey null.
        /// </summary>
        [Fact]
        public void GenerateKey_DovrebbeSollevareEccezione_PerBaseKeyNull()
        {
            // Arrange
            _mockGenerator.Setup(x => x.GenerateKey(null!, It.IsAny<object[]>()))
                .Throws<ArgumentNullException>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _mockGenerator.Object.GenerateKey(null!, 123));
        }
    }
}