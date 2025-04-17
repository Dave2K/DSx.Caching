using DSx.Caching.Abstractions.Health;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Health
{
    /// <summary>
    /// Test suite per verificare il corretto funzionamento di <see cref="CacheMetricsProvider"/>
    /// </summary>
    public class CacheMetricsProviderTests
    {
        /// <summary>
        /// Verifica che le metriche vengano accumulate correttamente durante le operazioni
        /// </summary>
        [Fact]
        public void GetCurrentMetrics_ShouldAccumulateCounters()
        {
            var provider = new CacheMetricsProvider();
            provider.RecordHit();
            provider.RecordHit();
            provider.RecordMiss();
            provider.RecordError();

            var metrics = provider.GetCurrentMetrics();

            metrics.TotalHits.Should().Be(2);
            metrics.TotalMisses.Should().Be(1);
            metrics.ErrorRate.Should().Be(0.25);
        }

        // Implementazione interna per testing
        internal class CacheMetricsProvider : ICacheMetricsProvider
        {
            private long _hits;
            private long _misses;
            private long _errors;

            public void RecordHit() => Interlocked.Increment(ref _hits);
            public void RecordMiss() => Interlocked.Increment(ref _misses);
            public void RecordError() => Interlocked.Increment(ref _errors);

            public CacheMetrics GetCurrentMetrics() => new()
            {
                TotalHits = _hits,
                TotalMisses = _misses,
                ErrorRate = (double)_errors / (_hits + _misses + _errors)
            };
        }
    }
}