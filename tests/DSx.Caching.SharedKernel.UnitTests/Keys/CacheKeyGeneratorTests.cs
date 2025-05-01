using DSx.Caching.SharedKernel.Constants;
using DSx.Caching.SharedKernel.Exceptions;
using DSx.Caching.SharedKernel.Validation;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Keys
{
    /// <summary>
    /// Test per la generazione e validazione delle chiavi di cache
    /// </summary>
    public class CacheKeyValidatorIntegrationTests
    {
        private readonly CacheKeyValidator _validator = new();

        /// <summary>
        /// Verifica il ciclo completo generazione-validazione-normalizzazione
        /// </summary>
        [Theory]
        [InlineData("Prodotto", new object[] { 123, "v2" }, "prodotto_123_v2")]
        public void FullKeyLifecycle_DovrebbeProdurreChiaveValida(
            string baseKey,
            object[] parameters,
            string expected)
        {
            // Arrange
            var rawKey = $"{baseKey}_{string.Join("_", parameters)}";

            // Act
            var normalizedKey = _validator.NormalizeKey(rawKey);
            _validator.Validate(normalizedKey);

            // Assert
            normalizedKey.Should().Be(expected);
            normalizedKey.Should().MatchRegex(CacheKeyConstants.KeyValidationPattern);
        }
    }
}
