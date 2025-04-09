using DSx.Caching.Abstractions.Validators;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="CacheKeyValidator"/>
    /// </summary>
    public class CacheKeyValidatorTests
    {
        /// <summary>
        /// Verifica il comportamento con chiavi valide
        /// </summary>
        /// <param name="key">Chiave da testare</param>
        [Theory]
        [InlineData("valid_key-123")]
        [InlineData("TEST_KEY")]
        [InlineData("a")]
        [InlineData("key_with_underscore")]
        [InlineData("12345")]
        public void ValidateKey_ChiaviValide_NonGeneraEccezioni(string key)
        {
            // Act
            var exception = Record.Exception(() => CacheKeyValidator.ThrowIfInvalid(key));

            // Assert
            exception.Should().BeNull("Chiave valida non dovrebbe generare eccezioni");
        }

        /// <summary>
        /// Verifica il comportamento con caratteri non permessi
        /// </summary>
        /// <param name="key">Chiave non valida da testare</param>
        [Theory]
        [InlineData("invalid!key")]
        [InlineData("key with spaces")]
        [InlineData("key?test")]
        [InlineData("key/with/slash")]
        public void ValidateKey_CaratteriNonPermessi_GeneraEccezione(string key)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key));

            ex.ParamName.Should().Be("key");
            ex.Message.Should().Contain("non valido");
        }

        /// <summary>
        /// Verifica il limite massimo di lunghezza della chiave
        /// </summary>
        [Fact]
        public void ValidateKey_LunghezzaMassima_NonGeneraEccezioni()
        {
            // Arrange
            var key = new string('a', 128);

            // Act & Assert
            CacheKeyValidator.ThrowIfInvalid(key);
        }

        /// <summary>
        /// Verifica il superamento della lunghezza massima
        /// </summary>
        [Fact]
        public void ValidateKey_LunghezzaEccessiva_GeneraEccezione()
        {
            // Arrange
            var key = new string('a', 129);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key))
                .ParamName.Should().Be("key");
        }

        /// <summary>
        /// Verifica il comportamento con chiave vuota
        /// </summary>
        [Fact]
        public void ValidateKey_ChiaveVuota_GeneraEccezione()
        {
            // Arrange
            var key = string.Empty;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key));

            ex.ParamName.Should().Be("key");
            ex.Message.Should().Contain("non può essere vuota");
        }
    }
}