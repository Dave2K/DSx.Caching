using DSx.Caching.Abstractions.Models;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Fornisce lo stato di salute del cluster Redis (Funzionalità Enterprise)
    /// </summary>
    public interface IClusterHealthProvider
    {
        /// <summary>
        /// Ottiene lo stato attuale del cluster Redis
        /// </summary>
        /// <returns>Lo stato del cluster: Healthy, Degraded o Unhealthy</returns>
        /// <remarks>
        /// Richiede connessione a un cluster Redis configurazione cluster-mode enabled
        /// </remarks>
        ClusterStatus GetClusterHealth();
    }
}