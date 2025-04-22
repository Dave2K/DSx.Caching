using System;
using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Factory per la creazione di provider di cache.
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Crea un'istanza del provider di cache specificato.
        /// </summary>
        /// <param name="name">Nome del provider (case-insensitive).</param>
        /// <returns>Istanza del provider.</returns>
        /// <exception cref="ArgumentNullException">Se name è null o vuoto.</exception>
        /// <exception cref="ArgumentException">Se il provider non esiste.</exception>
        ICacheProvider CreateProvider(string name);

        /// <summary>
        /// Restituisce la lista dei nomi dei provider disponibili.
        /// </summary>
        /// <returns>Lista dei nomi dei provider.</returns>
        IEnumerable<string> GetProviderNames();
    }
}