namespace DSx.Caching.Abstractions.Health
{
    /// <summary>
    /// Stati possibili di salute della cache.
    /// </summary>
    public enum CacheHealthStatus
    {
        /// <summary>
        /// Cache funzionante correttamente.
        /// </summary>
        Healthy,

        /// <summary>
        /// Cache funzionante con degradazione prestazioni.
        /// </summary>
        Degraded,

        /// <summary>
        /// Cache non funzionante.
        /// </summary>
        Unhealthy,

        /// <summary>
        /// Stato sconosciuto (da usare solo in fase di inizializzazione).
        /// </summary>
        Unknown
    }
}