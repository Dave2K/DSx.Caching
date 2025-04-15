using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Policies;

namespace DSx.Caching.SharedKernel.Policies
{
    /// <summary>
    /// Implementa una politica LRU (Least Recently Used) con scadenza temporale
    /// </summary>
    public class LeastRecentlyUsedPolicy : ICachePolicy
    {
        private readonly TimeSpan _maxAge;
        private readonly int _maxAccessCount;

        /// <summary>
        /// Inizializza una nuova istanza della politica LRU
        /// </summary>
        /// <param name="maxAge">Durata massima di conservazione</param>
        /// <param name="maxAccessCount">Numero massimo di accessi prima dello refresh</param>
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
            return entry.ReadCount % 10 == 0; // Refresh ogni 10 accessi
        }

        /// <inheritdoc/>
        public int CalculateRetentionPriority(CacheEntryDescriptor entry)
        {
            return entry.ReadCount;
        }
    }
}