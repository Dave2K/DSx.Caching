using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Policies;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Policies
{
    /// <summary>
    /// Test per la politica di rimozione LRU (Least Recently Used)
    /// </summary>
    public class LeastRecentlyUsedPolicyTests
    {
        /// <summary>
        /// Verifica che i candidati alla rimozione siano ordinati correttamente
        /// </summary>
        [Fact]
        public void GetEvictionCandidates_DovrebbeOrdinarePerDataAccesso()
        {
            // Arrange
            var entries = new List<CacheEntryDescriptor>
            {
                new("key1", DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(-3), null, null, 0),
                new("key2", DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(-1), null, null, 0),
                new("key3", DateTime.UtcNow.AddHours(-5), DateTime.UtcNow.AddHours(-5), null, null, 0)
            };

            var policy = new LeastRecentlyUsedPolicy(TimeSpan.FromHours(2));

            // Act
            var result = policy.GetEvictionCandidates(entries);

            // Assert
            result.Should().ContainInOrder("key3", "key1");
        }

        /// <summary>
        /// Verifica il rispetto del limite massimo di elementi da rimuovere
        /// </summary>
        [Fact]
        public void GetEvictionCandidates_DovrebbeRispettareIlLimiteMassimo()
        {
            // Arrange
            var entries = Enumerable.Range(1, 10)
                .Select(i => new CacheEntryDescriptor(
                    $"key{i}",
                    DateTime.UtcNow.AddHours(-i),
                    DateTime.UtcNow.AddHours(-i),
                    null,
                    null,
                    0))
                .ToList();

            var policy = new LeastRecentlyUsedPolicy(TimeSpan.FromMinutes(30), 3);

            // Act
            var result = policy.GetEvictionCandidates(entries);

            // Assert
            result.Should().HaveCount(3);
        }
    }
}
