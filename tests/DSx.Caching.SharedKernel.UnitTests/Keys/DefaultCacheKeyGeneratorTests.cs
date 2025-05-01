using DSx.Caching.SharedKernel.Keys;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Keys
{
    /// <summary>
    /// Test per la generazione e normalizzazione delle chiavi di cache
    /// </summary>
    public class DefaultCacheKeyGeneratorTests
    {
        private readonly DefaultCacheKeyGenerator _generator = new();

        /// <summary>
        /// Verifica la corretta combinazione di base key e parametri
        /// </summary>
        [Fact]
        public void GenerateKey_DovrebbeCombinareBaseKeyEParametri()
        {
            var result = _generator.GenerateKey("Prodotto", 123, "ABC");
            result.Should().Be("prodotto_123_abc");
        }

        /// <summary>
        /// Verifica la rimozione dei caratteri non validi
        /// </summary>
        [Fact]
        public void NormalizeKey_DovrebbeRimuovereCaratteriNonValid()
        {
            var result = _generator.NormalizeKey("key@with$special#chars");
            result.Should().Be("key-with-special-chars");
        }

        /// <summary>
        /// Verifica il sollevamento eccezione per base key nulla
        /// </summary>
        [Fact]
        public void GenerateKey_DovrebbeSollevareEccezione_PerBaseKeyNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _generator.GenerateKey(null!, []));
        }
    }
}
