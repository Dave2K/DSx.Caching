namespace DSx.Caching.Abstractions.Health
{
    /// <summary>
    /// Fornisce metodi per il monitoraggio delle metriche della cache.
    /// </summary>
    public interface ICacheMetricsProvider
    {
        /// <summary>
        /// Recupera le metriche attuali della cache.
        /// </summary>
        /// <returns>
        /// Istanza di <see cref="CacheMetrics"/> contenente:
        /// - ErrorRate: percentuale errori (0-1)
        /// - TotalHits: accessi riusciti
        /// - TotalMisses: accessi falliti
        /// </returns>
        CacheMetrics GetCurrentMetrics();
    }

    /// <summary>
    /// Contiene le metriche di prestazione della cache.
    /// </summary>
    public class CacheMetrics
    {
        /// <summary>
        /// Percentuale di errori (valore compreso tra 0 e 1).
        /// </summary>
        public double ErrorRate { get; set; }

        /// <summary>
        /// Numero totale di accessi riusciti.
        /// </summary>
        public long TotalHits { get; set; }

        /// <summary>
        /// Numero totale di accessi falliti.
        /// </summary>
        public long TotalMisses { get; set; }

        /// <summary>
        /// Calcola il rapporto tra accessi validi e totali.
        /// </summary>
        /// <returns>
        /// Valore tra 0 (tutti falliti) e 1 (tutti riusciti).
        /// </returns>
        public double CalculateHitRatio()
        {
            if (TotalHits + TotalMisses == 0)
                return 0;

            return (double)TotalHits / (TotalHits + TotalMisses);
        }
    }
}