using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce operazioni batch avanzate sulla cache.
    /// </summary>
    public interface IBulkCacheOperator
    {
        /// <summary>
        /// Memorizza multipli elementi nella cache in un'unica operazione.
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi da memorizzare</typeparam>
        /// <param name="items">Dizionario chiave-valore degli elementi</param>
        /// <param name="options">Opzioni di memorizzazione (opzionale)</param>
        /// <returns>Task che rappresenta l'operazione asincrona</returns>
        /// <exception cref="ArgumentNullException">Se items è null</exception>
        /// <exception cref="CacheException">Errore durante l'operazione bulk</exception>
        /// <example>
        /// <code>
        /// var items = new Dictionary&lt;string, object&gt; { {"key1", value1}, {"key2", value2} };
        /// await bulkOperator.BulkSetAsync(items, new CacheEntryOptions { AbsoluteExpiration = TimeSpan.FromHours(1) });
        /// </code>
        /// </example>
        Task BulkSetAsync<T>(IDictionary<string, T> items, CacheEntryOptions? options = null);

        /// <summary>
        /// Recupera multipli elementi dalla cache in un'unica operazione.
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi da recuperare</typeparam>
        /// <param name="keys">Lista delle chiavi da recuperare</param>
        /// <returns>Dizionario con gli elementi trovati</returns>
        Task<IDictionary<string, T>> BulkGetAsync<T>(IEnumerable<string> keys);
    }
}
