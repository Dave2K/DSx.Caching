using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce un servizio semplificato per l'interazione con la cache.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Recupera un valore dalla cache.
        /// </summary>
        /// <typeparam name="T">Tipo del valore.</typeparam>
        /// <param name="key">Chiave associata al valore.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache.
        /// </summary>
        /// <typeparam name="T">Tipo del valore.</typeparam>
        /// <param name="key">Chiave associata al valore.</param>
        /// <param name="value">Valore da memorizzare.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una voce dalla cache.
        /// </summary>
        /// <param name="key">Chiave della voce da rimuovere.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}