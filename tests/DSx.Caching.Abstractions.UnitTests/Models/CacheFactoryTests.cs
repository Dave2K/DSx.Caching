using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per l'interfaccia <see cref="ICacheFactory"/>
    /// </summary>
    public class CacheFactoryTests
    {
        /// <summary>
        /// Verifica che il metodo CreateProvider restituisca un'istanza valida quando il nome del provider è corretto
        /// </summary>
        [Fact]
        public void CreateProvider_ReturnsProvider_WhenNameIsValid()
        {
            // Arrange
            var mockProvider = new Mock<ICacheProvider>();
            var factory = new Mock<ICacheFactory>();

            factory
                .Setup(x => x.CreateProvider("Memory"))
                .Returns(mockProvider.Object);

            // Act
            var provider = factory.Object.CreateProvider("Memory");

            // Assert
            provider.Should().NotBeNull();
        }
    }
}