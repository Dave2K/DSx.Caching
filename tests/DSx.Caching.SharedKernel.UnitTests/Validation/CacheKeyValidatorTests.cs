using DSx.Caching.SharedKernel.Validation;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Validation
{
    /// <summary>
    /// Test per il validatore di chiavi di cache
    /// </summary>
    public class CacheKeyValidatorTests
    {
        private readonly ICacheKeyValidator _validatore = new CacheKeyValidator();

        /// <summary>
        /// Verifica il comportamento con chiavi valide
        /// </summary>
        [Theory]
        [InlineData("chiave_valida_123")]
        [InlineData("TEST-KEY")]
        public void Validate_ChiaveValida_NonGeneraEccezioni(string chiave)
        {
            // Act
            var eccezione = Record.Exception(() => _validatore.Validate(chiave));

            // Assert
            eccezione.Should().BeNull();
        }

        /// <summary>
        /// Verifica la normalizzazione delle chiavi
        /// </summary>
        [Fact]
        public void NormalizeKey_ConSpazi_RestituisceChiaveNormalizzata()
        {
            // Act
            var risultato = _validatore.NormalizeKey("  chiave con spazi  ");

            // Assert
            risultato.Should().Be("chiave-con-spazi");
        }
    }
}