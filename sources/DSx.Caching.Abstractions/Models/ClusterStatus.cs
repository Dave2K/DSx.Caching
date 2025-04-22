namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Rappresenta lo stato di salute del cluster Redis
    /// </summary>
    public enum ClusterStatus
    {
        /// <summary>
        /// Tutti i nodi sono operativi
        /// </summary>
        Healthy,

        /// <summary>
        /// Uno o più nodi non sono raggiungibili ma il cluster è operativo
        /// </summary>
        Degraded,

        /// <summary>
        /// Il cluster non è operativo
        /// </summary>
        Unhealthy
    }
}