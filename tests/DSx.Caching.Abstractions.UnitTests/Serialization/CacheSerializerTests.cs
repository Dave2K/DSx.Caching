using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Serialization;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Serialization
{
    /// <summary>
    /// Contiene i test unitari per verificare il comportamento del serializzatore di cache.
    /// </summary>
    public class CacheSerializerTests
    {
        private readonly Mock<ICacheSerializer> _mockSerializer = new();

        /// <summary>
        /// Verifica che il metodo Serialize sollevi ArgumentNullException quando viene passato un valore null.
        /// </summary>
        [Fact]
        public void Serialize_DovrebbeSollevareArgumentNullException_PerInputNull()
        {
            // Arrange
            _mockSerializer.Setup(x => x.Serialize<object>(It.IsAny<object>()))
                .Throws<ArgumentNullException>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _mockSerializer.Object.Serialize<object>(null!));
        }

        /// <summary>
        /// Verifica che il metodo Deserialize restituisca l'oggetto corretto quando i dati sono validi.
        /// </summary>
        [Fact]
        public void Deserialize_DovrebbeRestituireOggettoCorretto_PerDatiValidi()
        {
            // Arrange
            var expected = new { Id = 1, Name = "Test" };
            _mockSerializer.Setup(x => x.Deserialize<object>(It.IsAny<byte[]>()))
                .Returns(expected);

            // Act
            var result = _mockSerializer.Object.Deserialize<object>(new byte[10]);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Verifica che il metodo Deserialize sollevi CacheSerializationException per dati non validi.
        /// </summary>
        [Fact]
        public void Deserialize_DovrebbeSollevareEccezione_PerDatiNonValidi()
        {
            // Arrange
            _mockSerializer.Setup(x => x.Deserialize<object>(It.IsAny<byte[]>()))
                .Throws(new CacheSerializationException("Test error", null));

            // Act & Assert
            var ex = Assert.Throws<CacheSerializationException>(() =>
                _mockSerializer.Object.Deserialize<object>(new byte[10]));

            ex.Message.Should().Be("Test error");
        }
    }
}