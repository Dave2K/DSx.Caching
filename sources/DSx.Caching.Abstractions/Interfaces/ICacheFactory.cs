using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Factory per la creazione di provider di cache
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Crea un provider di cache specifico
        /// </summary>
        /// <param name="name">Nome del provider (es: "Redis", "Memory")</param>
        /// <returns>Istanza del provider</returns>
        ICacheProvider CreateProvider(string name);

        /// <summary>
        /// Restituisce i nomi dei provider disponibili
        /// </summary>
        /// <returns>Elenco dei nomi</returns>
        IEnumerable<string> GetProviderNames();
    }
}