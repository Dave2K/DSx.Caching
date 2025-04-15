using DSx.Caching.SharedKernel.Validation;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Validation
{
    /// <summary>
    /// Test suite per la validazione delle chiavi della cache
    /// </summary>
    public class CacheKeyValidatorTests
    {
        private readonly CacheKeyValidator _validator = new();

        /// <summary>
        /// Verifica l'accettazione di chiavi nel formato corretto
        /// </summary>
        /// <param name="key">Chiave da testare</param>
        [Theory]
        [InlineData("valid-key_123")]
        [InlineData("TEST-KEY")]
        public void Validate_DovrebbeAccettareChiaviValide(string key)
        {
            var act = () => _validator.Validate(key);
            act.Should().NotThrow();
        }

        /// <summary>
        /// Verifica il rifiuto di chiavi non conformi
        /// </summary>
        /// <param name="key">Chiave non valida</param>
        [Theory]
        [InlineData("invalid!key")]
        [InlineData("")]
        public void Validate_DovrebbeRifiutareChiaviNonValide(string key)
        {
            var act = () => _validator.Validate(key);
            act.Should().Throw<ArgumentException>();
        }
    }
}