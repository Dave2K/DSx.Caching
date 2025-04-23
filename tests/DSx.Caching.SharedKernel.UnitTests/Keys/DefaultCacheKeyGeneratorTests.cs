using DSx.Caching.Abstractions.Keys;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Keys
{
    /// <summary>
    /// Unit tests for DefaultCacheKeyGenerator implementation
    /// </summary>
    public class DefaultCacheKeyGeneratorTests
    {
        private readonly ICacheKeyGenerator _generator = new DefaultCacheKeyGenerator();

        /// <summary>
        /// Verifies key generation with valid parameters
        /// </summary>
        [Theory]
        [InlineData("product", new object[] { 123, "v2" }, "product_123_v2")]
        [InlineData("CATEGORY", new object[] { "items", 5 }, "category_items_5")]
        public void GenerateKey_ValidParameters_ReturnsNormalizedKey(
            string baseKey, object[] parameters, string expected)
        {
            // Act
            var result = _generator.GenerateKey(baseKey, parameters);

            // Assert
            result.Should().Be(expected);
        }

        /// <summary>
        /// Verifies key normalization handles special characters
        /// </summary>
        [Theory]
        [InlineData("Test@Key!", "test-key-")]
        [InlineData("  Spaces  Here  ", "spaces--here")]
        [InlineData("MixedCase123!", "mixedcase123-")]
        public void NormalizeKey_ProblematicInput_ReturnsValidFormat(
            string input, string expected)
        {
            // Act
            var result = _generator.NormalizeKey(input);

            // Assert
            result.Should().Be(expected);
            result.Should().MatchRegex(@"^[\w\-]{1,128}$");
        }

        /// <summary>
        /// Verifies exception for null base key
        /// </summary>
        [Fact]
        public void GenerateKey_NullBaseKey_ThrowsException()
        {
            // Arrange
            var act = () => _generator.GenerateKey(null!, new object[0]);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
