using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Servizio principale per l'interazione con la cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore memorizzato</typeparam>
        /// <param name="key">Chiave identificativa</param>
        /// <returns>Valore memorizzato o default</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave identificativa</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <returns>Task asincrono</returns>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Rimuove un elemento dalla cache
        /// </summary>
        /// <param name="key">Chiave identificativa</param>
        /// <returns>Task asincrono</returns>
        Task RemoveAsync(string key);
    }
}