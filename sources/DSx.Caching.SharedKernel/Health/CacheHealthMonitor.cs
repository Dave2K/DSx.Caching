using DSx.Caching.Abstractions.Health;

namespace DSx.Caching.SharedKernel.Health
{
    /// <inheritdoc cref="ICacheHealthMonitor"/>
    public class CacheHealthMonitor(ICacheMetricsProvider metricsProvider) : ICacheHealthMonitor
    {
        private readonly ICacheMetricsProvider _metricsProvider = metricsProvider;

        /// <inheritdoc/>
        public CacheHealthStatus GetHealthStatus()
        {
            var metrics = _metricsProvider.GetCurrentMetrics();
            return metrics.ErrorRate switch
            {
                < 0.01 => CacheHealthStatus.Healthy,
                < 0.1 => CacheHealthStatus.Degraded,
                _ => CacheHealthStatus.Unhealthy
            };
        }

        /// <inheritdoc/>
        public double CalculateCacheHitRatio()
        {
            var metrics = _metricsProvider.GetCurrentMetrics();
            return (double)metrics.TotalHits / (metrics.TotalHits + metrics.TotalMisses);
        }
    }
}