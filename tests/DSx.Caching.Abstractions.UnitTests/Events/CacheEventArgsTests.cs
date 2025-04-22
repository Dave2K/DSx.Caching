using DSx.Caching.Abstractions.Events;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Events
{
    /// <summary>
    /// Test per verificare il comportamento degli argomenti degli eventi della cache.
    /// </summary>
    public class CacheEventArgsTests
    {
        /// <summary>
        /// Verifica che il costruttore inizializzi correttamente le proprietà.
        /// </summary>
        [Fact]
        public void Costruttore_DovrebbeInizializzareProprieta()
        {
            // Arrange & Act
            var args = new CacheEventArgs(
                key: "test_key",
                operationType: CacheOperationType.Set,
                success: true,
                additionalInfo: "test_info");

            // Assert
            args.Key.Should().Be("test_key");
            args.OperationType.Should().Be(CacheOperationType.Set);
            args.Success.Should().BeTrue();
            args.AdditionalInfo.Should().Be("test_info");
        }

        /// <summary>
        /// Verifica che venga sollevata eccezione per chiave nulla.
        /// </summary>
        [Fact]
        public void Costruttore_DovrebbeSollevareEccezione_PerChiaveNulla()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CacheEventArgs(null!, CacheOperationType.Get));
        }

        /// <summary>
        /// Verifica i valori dell'enum CacheOperationType.
        /// </summary>
        [Fact]
        public void CacheOperationType_DovrebbeAvereValoriCorretti()
        {
            // Assert
            ((int)CacheOperationType.Get).Should().Be(0);
            ((int)CacheOperationType.Set).Should().Be(1);
            ((int)CacheOperationType.Update).Should().Be(6);
        }
    }
}