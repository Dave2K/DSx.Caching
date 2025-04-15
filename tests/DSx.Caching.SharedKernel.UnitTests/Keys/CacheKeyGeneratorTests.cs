using DSx.Caching.SharedKernel.Keys;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Keys
{
    /// <summary>
    /// Test per il generatore di chiavi di cache
    /// </summary>
    public class CacheKeyGeneratorTests
    {
        private readonly DefaultCacheKeyGenerator _generator = new();

        /// <summary>
        /// Verifica la corretta generazione di una chiave
        /// </summary>
        [Theory]
        [InlineData("Prodotto", new object[] { 123, "v2" }, "prodotto_123_v2")]
        public void GenerateKey_DovrebbeProdurreChiaveValida(
            string baseKey,
            object[] parameters,
            string expected)
        {
            var result = _generator.GenerateKey(baseKey, parameters);
            result.Should().Be(expected);
        }
    }
}