using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Fornisce un'interfaccia per le operazioni fondamentali di caching
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Ottiene un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore memorizzato</typeparam>
        /// <param name="key">Chiave identificativa del valore</param>
        /// <returns>Task che restituisce il valore trovato o default</returns>
        /// <exception cref="System.ArgumentException">Se la chiave non è valida</exception>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave identificativa</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <returns>Task che indica il completamento dell'operazione</returns>
        /// <exception cref="System.ArgumentNullException">Se il valore è null</exception>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Rimuove un valore dalla cache
        /// </summary>
        /// <param name="key">Chiave identificativa del valore da rimuovere</param>
        /// <returns>Task che indica il completamento dell'operazione</returns>
        /// <exception cref="System.ArgumentException">Se la chiave non è valida</exception>
        Task RemoveAsync(string key);
    }
}