// File: DSx.Caching.Providers.Memory/HealthChecks/MemoryCacheHealthCheck.cs

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DSx.Caching.Providers.Memory.HealthChecks
{
    /// <summary>
    /// Health check per la cache in memoria
    /// </summary>
    /// <param name="cache">Istanza della cache</param>
    public class MemoryCacheHealthCheck(IMemoryCache cache) : IHealthCheck
    {
        private readonly IMemoryCache _cache = cache;

        /// <summary>
        /// Esegue il controllo dello stato
        /// </summary>
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            const string testKey = "__HealthCheckTestKey__";
            _cache.Set(testKey, "OK", TimeSpan.FromSeconds(1));

            return Task.FromResult(
                _cache.TryGetValue(testKey, out string? value) && value == "OK"
                    ? HealthCheckResult.Healthy("OK")
                    : HealthCheckResult.Unhealthy("Test failed")
            );
        }
    }
}