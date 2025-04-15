namespace DSx.Caching.Abstractions.Health
{
    /// <summary>
    /// Fornisce il monitoraggio dello stato di salute della cache
    /// </summary>
    public interface ICacheHealthMonitor
    {
        /// <summary>
        /// Ottiene lo stato corrente della salute della cache
        /// </summary>
        CacheHealthStatus GetHealthStatus();

        /// <summary>
        /// Calcola il rapporto tra accessi validi e totali
        /// </summary>
        double CalculateCacheHitRatio();
    }
}