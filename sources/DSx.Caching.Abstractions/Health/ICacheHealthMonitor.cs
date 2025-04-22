namespace DSx.Caching.Abstractions.Health
{
    /// <summary>
    /// Fornisce il monitoraggio dello stato di salute della cache.
    /// </summary>
    public interface ICacheHealthMonitor
    {
        /// <summary>
        /// Ottiene lo stato corrente della salute della cache.
        /// </summary>
        /// <returns>
        /// Valore dell'enum <see cref="CacheHealthStatus"/> che indica lo stato:
        /// - Healthy: cache operativa
        /// - Degraded: prestazioni ridotte
        /// - Unhealthy: cache non funzionante
        /// </returns>
        CacheHealthStatus GetHealthStatus();

        /// <summary>
        /// Calcola il rapporto tra accessi validi e totali.
        /// </summary>
        /// <returns>
        /// Valore compreso tra 0 e 1 dove:
        /// - 1: tutti accessi validi (cache ottimale)
        /// - 0: tutti accessi falliti
        /// </returns>
        double CalculateCacheHitRatio();
    }
}