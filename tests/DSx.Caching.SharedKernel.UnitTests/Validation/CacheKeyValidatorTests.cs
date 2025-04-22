using DSx.Caching.SharedKernel.Validation;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Validation
{
    /// <summary>
    /// Test per il validatore di chiavi della cache
    /// </summary>
    public class CacheKeyValidatorTests
    {
        /// <summary>
        /// Verifica la normalizzazione delle chiavi speciali
        /// </summary>
        [Theory]
        [InlineData("Test@123!", "test-123-")]
        [InlineData("  Spaces  ", "spaces")]
        public void NormalizeKey_DovrebbePulireChiavi(string input, string expected)
        {
            var validator = new CacheKeyValidator();
            var result = validator.NormalizeKey(input);

            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifica il rifiuto di chiavi non valide
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid key with spaces")]
        public void Validate_DovrebbeRigettareChiaviNonValide(string? key)
        {
            var validator = new CacheKeyValidator();

            Assert.Throws<ArgumentException>(() =>
                validator.Validate(key!));
        }
    }
}