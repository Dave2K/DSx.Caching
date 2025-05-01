// File: DSx.Caching.Abstractions/Policies/LeastRecentlyUsedPolicy.cs
using DSx.Caching.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSx.Caching.Abstractions.Policies
{
    /// <summary>
    /// Implementa una politica di rimozione basata sull'uso meno recente
    /// </summary>
    public class LeastRecentlyUsedPolicy : ICacheEvictionPolicy
    {
        private readonly TimeSpan _maxInactiveTime;
        private readonly int _maxItemsToEvict;

        /// <summary>
        /// Inizializza una nuova istanza della politica LRU
        /// </summary>
        /// <param name="maxInactiveTime">Tempo massimo di inattività</param>
        /// <param name="maxItemsToEvict">Numero massimo di elementi da rimuovere</param>
        public LeastRecentlyUsedPolicy(TimeSpan maxInactiveTime, int maxItemsToEvict = 0)
        {
            _maxInactiveTime = maxInactiveTime;
            _maxItemsToEvict = maxItemsToEvict;
        }

        /// <summary>
        /// Ottiene la lista delle chiavi candidate alla rimozione
        /// </summary>
        /// <param name="entries">Elenco delle voci della cache</param>
        /// <returns>Chiavi candidate alla rimozione</returns>
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
