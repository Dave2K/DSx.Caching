using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Fornisce un'interfaccia semplificata per le operazioni di caching
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave da recuperare</param>
        /// <returns>Valore memorizzato</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore</typeparam>
        /// <param name="key">Chiave da impostare</param>
        /// <param name="value">Valore da memorizzare</param>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Rimuove un valore dalla cache
        /// </summary>
        /// <param name="key">Chiave da rimuovere</param>
        Task RemoveAsync(string key);
    }
}