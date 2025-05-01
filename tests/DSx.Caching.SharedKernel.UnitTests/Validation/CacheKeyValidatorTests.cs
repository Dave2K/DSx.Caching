using DSx.Caching.SharedKernel.Constants;
using DSx.Caching.SharedKernel.Interfaces;
using DSx.Caching.SharedKernel.Validation;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Validation
{
    /// <summary>
    /// Test suite per la validazione e normalizzazione delle chiavi di cache
    /// </summary>
    public class CacheKeyValidatorTests
    {
        private readonly CacheKeyValidator _validator = new();

        /// <summary>
        /// Verifica il successo della validazione per chiavi conformi
        /// </summary>
        /// <param name="key">Chiave da testare</param>
        [Theory]
        [InlineData("valid-key_123")]
        [InlineData("VALID_KEY")]
        [InlineData("1234567890")]
        public void Validate_ChiaveValida_DovrebbePassare(string key)
        {
            var exception = Record.Exception(() => _validator.Validate(key));
            exception.Should().BeNull();
        }

        /// <summary>
        /// Verifica il corretto sollevamento di eccezioni per chiavi non valide
        /// </summary>
        /// <param name="key">Chiave non valida</param>
        /// <param name="expectedMessage">Messaggio di errore atteso</param>
        [Theory]
        [InlineData("invalid key!", "Formato chiave non valido")]
        [InlineData("", "Formato chiave non valido")]
        [InlineData("   ", "Formato chiave non valido")]
        [InlineData("key@with#symbols", "Formato chiave non valido")]
        public void Validate_ChiaveNonValida_DovrebbeSollevareEccezione(string key, string expectedMessage)
        {
            var action = () => _validator.Validate(key);

            action.Should().Throw<InvalidCacheKeyException>()
                .WithMessage($"*{expectedMessage}*")
                .Which.InvalidKey.Should().Be(key);
        }

        /// <summary>
        /// Verifica la corretta normalizzazione delle chiavi
        /// </summary>
        /// <param name="input">Chiave da normalizzare</param>
        /// <param name="expected">Risultato atteso</param>
        [Theory]
        [InlineData("Test_Key@123", "test-key-123")]
        [InlineData("  TrimMe  ", "trimme")]
        [InlineData("UPPER_CASE", "upper-case")]
        [InlineData("very_long_key_that_exceeds_maximum_length_1234567890_1234567890_1234567890_1234567890_1234567890",
            "very-long-key-that-exceeds-maximum-length-1234567890-1234567890-1234567890-1234567890-1234567890")]
        public void NormalizeKey_DovrebbeRestituireChiaveCorretta(string input, string expected)
        {
            var result = _validator.NormalizeKey(input);

            result.Should().Be(expected.ToLowerInvariant());
            result.Length.Should().BeLessThanOrEqualTo(CacheKeyConstants.MaxKeyLength);
        }

        /// <summary>
        /// Verifica il troncamento delle chiavi oltre la lunghezza massima
        /// </summary>
        [Fact]
        public void NormalizeKey_ChiaveLunga_DovrebbeTroncare()
        {
            var longKey = new string('a', CacheKeyConstants.MaxKeyLength + 50);
            var result = _validator.NormalizeKey(longKey);

            result.Length.Should().Be(CacheKeyConstants.MaxKeyLength);
            result.Should().Be(longKey.ToLowerInvariant()[..CacheKeyConstants.MaxKeyLength]);
        }
    }
}
