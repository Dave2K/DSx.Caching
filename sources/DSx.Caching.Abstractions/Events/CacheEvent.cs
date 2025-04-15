using Microsoft.Extensions.Logging;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Eventi standard della cache
    /// </summary>
    public static class CacheEvent
    {
        /// <summary>
        /// Evento di cache miss
        /// </summary>
        public static readonly EventId CacheMiss = new(1001, "Cache Miss");

        /// <summary>
        /// Evento di scadenza cache
        /// </summary>
        public static readonly EventId CacheExpired = new(1002, "Cache Expired");
    }
}