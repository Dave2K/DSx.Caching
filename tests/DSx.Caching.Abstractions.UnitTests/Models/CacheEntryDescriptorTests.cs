using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per verificare il comportamento della classe CacheEntryDescriptor.
    /// </summary>
    public class CacheEntryDescriptorTests
    {
        /// <summary>
        /// Verifica che l'aggiornamento dopo una lettura incrementi il contatore.
        /// </summary>
        [Fact]
        public void UpdateOnRead_ShouldIncrementReadCountAndUpdateTimestamp()
        {
            // Arrange
            var originalTime = DateTime.UtcNow.AddMinutes(-5);
            var descriptor = new CacheEntryDescriptor("test_key", originalTime, 2, 1024, false);

            // Act
            descriptor.UpdateOnRead();

            // Assert
            descriptor.ReadCount.Should().Be(3);
            descriptor.LastAccessed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            descriptor.IsDirty.Should().BeFalse();
        }

        /// <summary>
        /// Verifica che l'aggiornamento dopo una scrittura modifichi la dimensione.
        /// </summary>
        [Fact]
        public void UpdateOnWrite_ShouldUpdateSizeAndMarkAsDirty()
        {
            // Arrange
            var descriptor = new CacheEntryDescriptor("test_key", DateTime.UtcNow, 0, 512, false);

            // Act
            descriptor.UpdateOnWrite(2048);

            // Assert
            descriptor.SizeInBytes.Should().Be(2048);
            descriptor.IsDirty.Should().BeTrue();
            descriptor.LastAccessed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Verifica che il metodo MarkAsClean resetti il flag di modifica.
        /// </summary>
        [Fact]
        public void MarkAsClean_ShouldResetDirtyFlag()
        {
            // Arrange
            var descriptor = new CacheEntryDescriptor("test_key", DateTime.UtcNow, 0, 256, true);

            // Act
            descriptor.MarkAsClean();

            // Assert
            descriptor.IsDirty.Should().BeFalse();
        }
    }
}