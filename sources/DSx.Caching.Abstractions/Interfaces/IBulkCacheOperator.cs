using DSx.Caching.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce operazioni batch avanzate sulla cache (Funzionalità Enterprise)
    /// </summary>
    public interface IBulkCacheOperator
    {
        /// <summary>
        /// Memorizza multipli elementi nella cache in un'unica operazione
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi da memorizzare</typeparam>
        /// <param name="items">Dizionario chiave-valore degli elementi</param>
        /// <param name="options">Opzioni di memorizzazione (opzionale)</param>
        /// <returns>Task che rappresenta l'operazione asincrona</returns>
        Task BulkSetAsync<T>(IDictionary<string, T> items, CacheEntryOptions? options = null);

        /// <summary>
        /// Recupera multipli elementi dalla cache in un'unica operazione
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi da recuperare</typeparam>
        /// <param name="keys">Lista delle chiavi da recuperare</param>
        /// <returns>Dizionario con gli elementi trovati</returns>
        Task<IDictionary<string, T>> BulkGetAsync<T>(IEnumerable<string> keys);
    }
}