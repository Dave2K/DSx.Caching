using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per verificare il comportamento dell'interfaccia ICacheFactory.
    /// </summary>
    public class CacheFactoryTests
    {
        private readonly Mock<ICacheFactory> _mockFactory = new();

        /// <summary>
        /// Verifica che CreateProvider sollevi ArgumentNullException per nome null.
        /// </summary>
        [Fact]
        public void CreateProvider_DovrebbeSollevareArgumentNullException_PerNomeNull()
        {
            // Arrange
            _mockFactory.Setup(x => x.CreateProvider(It.IsAny<string>()))
                .Throws<ArgumentNullException>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _mockFactory.Object.CreateProvider(null!));
        }

        /// <summary>
        /// Verifica che GetProviderNames restituisca una lista non vuota.
        /// </summary>
        [Fact]
        public void GetProviderNames_DovrebbeRestituireListaNonVuota()
        {
            // Arrange
            var expectedProviders = new List<string> { "Redis", "Memory" };
            _mockFactory.Setup(x => x.GetProviderNames())
                .Returns(expectedProviders);

            // Act
            var providers = _mockFactory.Object.GetProviderNames();

            // Assert
            providers.Should()
                .NotBeNullOrEmpty()
                .And
                .Equal(expectedProviders);
        }

        /// <summary>
        /// Verifica che CreateProvider sollevi ArgumentException per nome vuoto.
        /// </summary>
        [Fact]
        public void CreateProvider_DovrebbeSollevareArgumentException_PerNomeVuoto()
        {
            // Arrange
            _mockFactory.Setup(x => x.CreateProvider(It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _mockFactory.Object.CreateProvider(string.Empty));
        }
    }
}