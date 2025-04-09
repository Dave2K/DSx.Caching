using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="CacheKeyValidator"/>
    /// </summary>
    public class CacheKeyValidatorTests
    {
        private readonly ICacheKeyValidator _validator = new CacheKeyValidator();

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
        public void Validate_ChiaviValide_NonGeneraEccezioni(string key)
        {
            // Act
            var exception = Record.Exception(() => _validator.Validate(key));

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
        public void Validate_CaratteriNonPermessi_GeneraEccezione(string key)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.Validate(key));
        }

        /// <summary>
        /// Verifica il limite massimo di lunghezza della chiave
        /// </summary>
        [Fact]
        public void Validate_LunghezzaMassima_NonGeneraEccezioni()
        {
            // Arrange
            var key = new string('a', 128);

            // Act & Assert
            _validator.Validate(key);
        }

        /// <summary>
        /// Verifica il superamento della lunghezza massima
        /// </summary>
        [Fact]
        public void Validate_LunghezzaEccessiva_GeneraEccezione()
        {
            // Arrange
            var key = new string('a', 129);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.Validate(key));
        }

        /// <summary>
        /// Verifica il comportamento con chiave vuota
        /// </summary>
        [Fact]
        public void Validate_ChiaveVuota_GeneraEccezione()
        {
            // Arrange
            var key = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.Validate(key));
        }

        /// <summary>
        /// Verifica la corretta formattazione dei messaggi di errore
        /// </summary>
        [Fact]
        public void Validate_MessaggioErrore_ContieneDettagli()
        {
            // Arrange
            const string key = "chiave non valida!";

            // Act
            var ex = Assert.Throws<ArgumentException>(() => _validator.Validate(key));

            // Assert
            ex.Message.Should().Contain("Caratteri permessi: A-Z, a-z, 0-9, -, _");
        }
    }
}