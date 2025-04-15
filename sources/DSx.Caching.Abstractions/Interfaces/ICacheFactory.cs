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
        /// <param name="name">Nome del provider configurato</param>
        /// <returns>Istanza del provider richiesto</returns>
        /// <exception cref="System.ArgumentException">
        /// Sollevata se il nome del provider non è valido
        /// </exception>
        ICacheProvider CreateProvider(string name);

        /// <summary>
        /// Elenca i nomi dei provider disponibili
        /// </summary>
        /// <returns>Lista dei provider configurati</returns>
        IEnumerable<string> GetProviderNames();
    }
}