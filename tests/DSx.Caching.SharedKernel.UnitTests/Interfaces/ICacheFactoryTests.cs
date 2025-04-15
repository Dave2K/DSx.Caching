using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.SharedKernel.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Interfaces
{
    /// <summary>
    /// Test per l'interfaccia <see cref="ICacheFactory"/>.
    /// </summary>
    public class CacheFactoryTests
    {
        /// <summary>
        /// Verifica che <see cref="ICacheFactory.GetProviderNames"/> restituisca i provider configurati.
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
    }
}