using DSx.Caching.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSx.Caching.Abstractions.Policies
{
    /// <summary>
    /// Implementa una politica di rimozione basata sull'uso meno recente (LRU).
    /// </summary>
    public class LeastRecentlyUsedPolicy : ICacheEvictionPolicy
    {
        private readonly TimeSpan _maxInactiveTime;
        private readonly int _maxItemsToEvict;

        /// <summary>
        /// Inizializza una nuova istanza della politica LRU.
        /// </summary>
        /// <param name="maxInactiveTime">Durata massima di inattività prima della rimozione.</param>
        /// <param name="maxItemsToEvict">Numero massimo di elementi da rimuovere (0 = illimitato).</param>
        public LeastRecentlyUsedPolicy(TimeSpan maxInactiveTime, int maxItemsToEvict = 0)
        {
            _maxInactiveTime = maxInactiveTime;
            _maxItemsToEvict = maxItemsToEvict;
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetEvictionCandidates(IEnumerable<CacheEntryDescriptor> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            var cutoff = DateTime.UtcNow - _maxInactiveTime;
            var candidates = entries
                .Where(e => e.LastAccessed < cutoff)
                .OrderBy(e => e.LastAccessed)
                .Select(e => e.Key);

            return _maxItemsToEvict > 0
                ? candidates.Take(_maxItemsToEvict)
                : candidates;
        }
    }
}