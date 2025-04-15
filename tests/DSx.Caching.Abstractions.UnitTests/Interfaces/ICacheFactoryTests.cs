using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per ICacheFactory
    /// </summary>
    public class CacheFactoryTests
    {
        private readonly Mock<ICacheFactory> _mockFactory = new();

        /// <summary>
        /// Verifica la creazione di un provider valido
        /// </summary>
        [Fact]
        public void CreateProvider_DovrebbeRestituireProvider_ConNomeValido()
        {
            // Arrange
            var expectedProvider = new Mock<ICacheProvider>().Object;
            _mockFactory.Setup(x => x.CreateProvider("valid"))
                .Returns(expectedProvider);

            // Act
            var provider = _mockFactory.Object.CreateProvider("valid");

            // Assert
            provider.Should().BeSameAs(expectedProvider);
        }

        /// <summary>
        /// Verifica l'eccezione per provider non trovato
        /// </summary>
        [Fact]
        public void CreateProvider_DovrebbeSollevareEccezione_ConNomeNonValido()
        {
            // Arrange
            _mockFactory.Setup(x => x.CreateProvider("invalid"))
                .Throws(new ArgumentException("Provider non valido"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _mockFactory.Object.CreateProvider("invalid"));
        }

        /// <summary>
        /// Verifica l'elenco dei provider disponibili
        /// </summary>
        [Fact]
        public void GetProviderNames_DovrebbeRestituireListaConfigurata()
        {
            // Arrange
            var expected = new List<string> { "redis", "memory" };
            _mockFactory.Setup(x => x.GetProviderNames()).Returns(expected);

            // Act
            var providers = _mockFactory.Object.GetProviderNames();

            // Assert
            providers.Should().Equal(expected);
        }
    }
}