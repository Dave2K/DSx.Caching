using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Clustering
{
    /// <summary>
    /// Definisce le operazioni per la gestione di un cluster di cache distribuite.
    /// </summary>
    public interface ICacheClusterClient
    {
        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster.
        /// </summary>
        /// <param name="key">Chiave da invalidare.</param>
        /// <returns>Task asincrono.</returns>
        Task BroadcastInvalidationAsync(string key);

        /// <summary>
        /// Invalida tutte le chiavi corrispondenti a un pattern specificato.
        /// </summary>
        /// <param name="pattern">Pattern per filtrare le chiavi (es. "user_*").</param>
        /// <returns>Task asincrono.</returns>
        Task InvalidateByPatternAsync(string pattern);

        /// <summary>
        /// Sincronizza lo stato del cluster tra tutti i nodi.
        /// </summary>
        /// <returns>Task asincrono.</returns>
        Task SyncClusterStateAsync();
    }
}