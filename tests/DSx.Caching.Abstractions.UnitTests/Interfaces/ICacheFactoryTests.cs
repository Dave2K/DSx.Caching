using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per <see cref="ICacheFactory"/>
    /// </summary>
    public class CacheFactoryTests
    {
        /// <summary>
        /// Verifica che GetProviderNames restituisca i provider configurati
        /// </summary>
        [Fact]
        public void GetProviderNames_ReturnsConfiguredProviders()
        {
            // Arrange
            var expectedProviders = new List<string> { "Redis", "Memory" };
            var mockFactory = new Mock<ICacheFactory>();
            mockFactory.Setup(x => x.GetProviderNames()).Returns(expectedProviders);

            // Act
            var providers = mockFactory.Object.GetProviderNames();

            // Assert
            providers.Should().BeEquivalentTo(expectedProviders);
        }

        /// <summary>
        /// Verifica che CreateProvider sollevi un'eccezione per nomi non validi
        /// </summary>
        [Fact]
        public void CreateProvider_ThrowsException_ForInvalidName()
        {
            // Arrange
            var mockFactory = new Mock<ICacheFactory>();
            mockFactory
                .Setup(x => x.CreateProvider("Invalid"))
                .Throws(new ArgumentException("Provider non supportato"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mockFactory.Object.CreateProvider("Invalid"));
        }
    }
}