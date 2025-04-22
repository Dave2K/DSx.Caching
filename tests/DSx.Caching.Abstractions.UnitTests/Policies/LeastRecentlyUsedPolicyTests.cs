using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Policies;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Policies
{
    /// <summary>
    /// Contiene i test per la classe <see cref="LeastRecentlyUsedPolicy"/>
    /// </summary>
    public class LeastRecentlyUsedPolicyTests
    {
        /// <summary>
        /// Verifica l'eviction per voci più vecchie della durata massima
        /// </summary>
        [Fact]
        public void ShouldEvict_WhenEntryExceedsMaxAge()
        {
            // Arrange
            var policy = new LeastRecentlyUsedPolicy(TimeSpan.FromHours(1), 100);
            var oldEntry = new CacheEntryDescriptor(
                DateTime.UtcNow.AddHours(-2), 5, 1024, false);

            // Act
            var result = policy.ShouldEvict(oldEntry);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Verifica l'eviction per voci con troppi accessi
        /// </summary>
        [Fact]
        public void ShouldEvict_WhenEntryExceedsMaxAccessCount()
        {
            // Arrange
            var policy = new LeastRecentlyUsedPolicy(TimeSpan.MaxValue, 10);
            var activeEntry = new CacheEntryDescriptor(
                DateTime.UtcNow, 15, 2048, false);

            // Act
            var result = policy.ShouldEvict(activeEntry);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Verifica il calcolo corretto della priorità di ritenzione
        /// </summary>
        [Fact]
        public void CalculateRetentionPriority_ShouldReturnReadCount()
        {
            // Arrange
            var policy = new LeastRecentlyUsedPolicy(TimeSpan.MaxValue, 100);
            var entry = new CacheEntryDescriptor(DateTime.UtcNow, 25, 512, true);

            // Act
            var priority = policy.CalculateRetentionPriority(entry);

            // Assert
            Assert.Equal(entry.ReadCount, priority);
        }
    }
}