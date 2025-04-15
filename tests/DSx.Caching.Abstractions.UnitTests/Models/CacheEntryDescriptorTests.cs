using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Contiene i test unitari per la classe <see cref="CacheEntryDescriptor"/>
    /// </summary>
    public class CacheEntryDescriptorTests
    {
        /// <summary>
        /// Verifica che l'aggiornamento dopo una lettura incrementi il contatore
        /// e aggiorni il timestamp correttamente
        /// </summary>
        [Fact]
        public void UpdateOnRead_ShouldIncrementReadCountAndUpdateTimestamp()
        {
            // Arrange
            var originalTime = DateTime.UtcNow.AddMinutes(-5);
            var descriptor = new CacheEntryDescriptor(originalTime, 2, 1024, false);

            // Act
            descriptor.UpdateOnRead();

            // Assert
            descriptor.ReadCount.Should().Be(3);
            descriptor.LastAccessed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            descriptor.IsDirty.Should().BeFalse();
        }

        /// <summary>
        /// Verifica che l'aggiornamento dopo una scrittura modifichi la dimensione,
        /// imposti il flag di modifica e aggiorni il timestamp
        /// </summary>
        [Fact]
        public void UpdateOnWrite_ShouldUpdateSizeAndMarkAsDirty()
        {
            // Arrange
            var descriptor = new CacheEntryDescriptor(DateTime.UtcNow, 0, 512, false);

            // Act
            descriptor.UpdateOnWrite(2048);

            // Assert
            descriptor.SizeInBytes.Should().Be(2048);
            descriptor.IsDirty.Should().BeTrue();
            descriptor.LastAccessed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Verifica che il metodo MarkAsClean resetti correttamente il flag di modifica
        /// </summary>
        [Fact]
        public void MarkAsClean_ShouldResetDirtyFlag()
        {
            // Arrange
            var descriptor = new CacheEntryDescriptor(DateTime.UtcNow, 0, 256, true);

            // Act
            descriptor.MarkAsClean();

            // Assert
            descriptor.IsDirty.Should().BeFalse();
        }
    }
}