using DSx.Caching.Abstractions.Validators;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Test suite per la validazione delle chiavi della cache
    /// </summary>
    public class CacheKeyValidatorTests
    {
        /// <summary>
        /// Verifica che chiavi valide non generino eccezioni
        /// </summary>
        /// <param name="key">Chiave da testare</param>
        [Theory]
        [InlineData("valid_key-123")]
        [InlineData("TEST_KEY")]
        [InlineData("a")]
        [InlineData("key_with_underscore")]
        [InlineData("12345")]
        public void ValidateKey_ValidKeys_ShouldNotThrow(string key)
        {
            // Act
            var exception = Record.Exception(() => CacheKeyValidator.ThrowIfInvalid(key));

            // Assert
            exception.Should().BeNull("Chiave valida non dovrebbe generare eccezioni");
        }

        /// <summary>
        /// Verifica che chiavi con caratteri non consentiti generino eccezioni
        /// </summary>
        /// <param name="key">Chiave non valida da testare</param>
        [Theory]
        [InlineData("invalid!key")]
        [InlineData("key with spaces")]
        [InlineData("key?test")]
        [InlineData("key/with/slash")]
        public void ValidateKey_InvalidCharacters_ShouldThrow(string key)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key));

            ex.ParamName.Should().Be("key");
            ex.Message.Should().Contain("non valido", "Messaggio d'errore appropriato");
        }

        /// <summary>
        /// Verifica il limite massimo di lunghezza della chiave (128 caratteri)
        /// </summary>
        [Fact]
        public void ValidateKey_MaxLengthBoundary_ShouldNotThrow()
        {
            // Arrange
            var key = new string('a', 128);

            // Act & Assert
            CacheKeyValidator.ThrowIfInvalid(key);
        }

        /// <summary>
        /// Verifica che una chiave superiore alla lunghezza massima generi un'eccezione
        /// </summary>
        [Fact]
        public void ValidateKey_OverMaxLength_ShouldThrow()
        {
            // Arrange
            var key = new string('a', 129);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key))
                .ParamName.Should().Be("key");
        }

        /// <summary>
        /// Verifica che una chiave vuota generi un'eccezione
        /// </summary>
        [Fact]
        public void ValidateKey_EmptyKey_ShouldThrow()
        {
            // Arrange
            var key = string.Empty;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                CacheKeyValidator.ThrowIfInvalid(key));

            ex.ParamName.Should().Be("key");
            ex.Message.Should().Contain("non può essere vuota", "Messaggio d'errore appropriato");
        }
    }
}