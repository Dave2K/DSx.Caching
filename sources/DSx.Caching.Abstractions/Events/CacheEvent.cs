using Microsoft.Extensions.Logging;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Eventi standard della cache con relativi ID per il logging.
    /// </summary>
    public static class CacheEvent
    {
        /// <summary>
        /// Evento di cache miss (elemento non trovato).
        /// </summary>
        public static readonly EventId CacheMiss = new(1001, "Cache Miss");

        /// <summary>
        /// Evento di scadenza cache (elemento rimosso per expiration).
        /// </summary>
        public static readonly EventId CacheExpired = new(1002, "Cache Expired");

        /// <summary>
        /// Evento di conflitto di concorrenza.
        /// </summary>
        public static readonly EventId ConcurrencyConflict = new(1003, "Concurrency Conflict");

        /// <summary>
        /// Evento di errore durante la serializzazione.
        /// </summary>
        public static readonly EventId SerializationError = new(1004, "Serialization Error");
    }
}