using DSx.Caching.Abstractions.Models;
using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Policies
{
    /// <summary>
    /// Definisce la politica di rimozione elementi dalla cache.
    /// </summary>
    public interface ICacheEvictionPolicy
    {
        /// <summary>
        /// Determina le chiavi candidate alla rimozione.
        /// </summary>
        /// <param name="entries">Elenco di voci della cache con relativi metadati.</param>
        /// <returns>
        /// Sequenza ordinata di chiavi da rimuovere (priorità più alta per prime).
        /// </returns>
        IEnumerable<string> GetEvictionCandidates(IEnumerable<CacheEntryDescriptor> entries);
    }
}