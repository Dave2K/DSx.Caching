using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Interfaccia factory per la creazione di istanze di provider di cache
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Crea un'istanza di provider di cache con nome specificato
        /// </summary>
        /// <param name="name">Nome logico del provider</param>
        /// <returns>Istanza configurata del provider di cache</returns>
        ICacheProvider CreateProvider(string name);

        /// <summary>
        /// Ottiene tutti i nomi dei provider registrati
        /// </summary>
        /// <returns>Collezione dei nomi dei provider disponibili</returns>
        IEnumerable<string> GetProviderNames();
    }
}