using System;

namespace DSx.Caching.Abstractions.Resiliency
{
    /// <summary>
    /// Opzioni di configurazione per il circuit breaker
    /// </summary>
    public class CacheCircuitBreakerOptions
    {
        /// <summary>
        /// Soglia di fallimenti prima di aprire il circuito
        /// </summary>
        public int FailureThreshold { get; set; } = 5;

        /// <summary>
        /// Durata dell'apertura del circuito
        /// </summary>
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromMinutes(1);
    }
}