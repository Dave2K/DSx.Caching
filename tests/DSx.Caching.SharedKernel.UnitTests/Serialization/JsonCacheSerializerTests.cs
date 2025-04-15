using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.SharedKernel.Serialization;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Serialization
{
    /// <summary>
    /// Test suite per la serializzazione JSON
    /// </summary>
    public class JsonCacheSerializerTests
    {
        private readonly JsonCacheSerializer _serializer = new();

        // Aggiungi DTO per il test
        private class TestDto
        {
            public string Name { get; set; } = null!;
            public int Value { get; set; }
        }

        /// <summary>
        /// Verifica il ciclo completo di serializzazione/deserializzazione
        /// </summary>
        [Fact]
        public void Serialize_Deserialize_DovrebberoEssereReversibili()
        {
            // Arrange
            var original = new TestDto { Name = "Test", Value = 123 };

            // Act
            var bytes = _serializer.Serialize(original);
            var result = _serializer.Deserialize<TestDto>(bytes);

            // Assert
            result.Name.Should().Be(original.Name);
            result.Value.Should().Be(original.Value);
        }

        /// <summary>
        /// Verifica la gestione degli errori di deserializzazione
        /// </summary>
        [Fact]
        public void Deserialize_DovrebbeSollevareEccezione_ConDatiInvalidi()
        {
            // Arrange
            var invalidData = new byte[] { 0x00, 0x01 };

            // Act & Assert
            Assert.Throws<CacheSerializationException>(
                () => _serializer.Deserialize<TestDto>(invalidData)
            );
        }
    }
}