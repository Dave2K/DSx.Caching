using DSx.Caching.Abstractions.Models;

namespace DSx.Caching.Abstractions.Policies
{
    /// <summary>
    /// Implementa una politica di rimozione basata sull'uso meno recente (LRU).
    /// </summary>
    public class LeastRecentlyUsedPolicy : ICachePolicy
    {
        private readonly TimeSpan _maxAge;
        private readonly int _maxAccessCount;

        /// <summary>
        /// Inizializza una nuova istanza della politica LRU.
        /// </summary>
        /// <param name="maxAge">Durata massima di validità di una voce nella cache.</param>
        /// <param name="maxAccessCount">Numero massimo di accessi consentiti prima della rimozione.</param>
        public LeastRecentlyUsedPolicy(TimeSpan maxAge, int maxAccessCount)
        {
            _maxAge = maxAge;
            _maxAccessCount = maxAccessCount;
        }

        /// <inheritdoc/>
        public bool ShouldEvict(CacheEntryDescriptor entry)
        {
            var age = DateTime.UtcNow - entry.LastAccessed;
            return age > _maxAge || entry.ReadCount > _maxAccessCount;
        }

        /// <inheritdoc/>
        public bool ShouldRefresh(CacheEntryDescriptor entry)
        {
            return entry.ReadCount % 10 == 0;
        }

        /// <inheritdoc/>
        public int CalculateRetentionPriority(CacheEntryDescriptor entry)
        {
            return entry.ReadCount;
        }
    }
}