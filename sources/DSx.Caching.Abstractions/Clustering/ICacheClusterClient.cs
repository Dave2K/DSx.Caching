using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Clustering
{
    /// <summary>
    /// Client per il coordinamento di cache distribuite
    /// </summary>
    public interface ICacheClusterClient
    {
        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster
        /// </summary>
        /// <param name="key">Chiave da invalidare</param>
        Task BroadcastInvalidationAsync(string key);

        /// <summary>
        /// Sincronizza lo stato della cache tra i nodi
        /// </summary>
        Task SyncClusterStateAsync();
    }
}