namespace DSx.Caching.Abstractions.Health
{
    /// <summary>
    /// Fornisce metodi per il monitoraggio delle metriche della cache
    /// </summary>
    public interface ICacheMetricsProvider
    {
        /// <summary>
        /// Recupera le metriche attuali della cache
        /// </summary>
        /// <returns>Un'istanza di <see cref="CacheMetrics"/> contenente i dati delle prestazioni</returns>
        CacheMetrics GetCurrentMetrics();
    }

    /// <summary>
    /// Contiene le metriche di prestazione della cache
    /// </summary>
    public class CacheMetrics
    {
        /// <summary>
        /// Percentuale di errori (valore compreso tra 0 e 1)
        /// </summary>
        public double ErrorRate { get; set; }

        /// <summary>
        /// Numero totale di accessi riusciti
        /// </summary>
        public long TotalHits { get; set; }

        /// <summary>
        /// Numero totale di accessi falliti
        /// </summary>
        public long TotalMisses { get; set; }
    }
}