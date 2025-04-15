using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Opzioni configurabili per una entry della cache
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Durata assoluta prima della scadenza
        /// </summary>
        /// <value>Intervallo di tempo nullable</value>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Durata di sliding prima della scadenza
        /// </summary>
        /// <value>Intervallo di tempo nullable</value>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Priorità di ritenzione nella cache
        /// </summary>
        /// <value>Valore dell'enum CacheEntryPriority</value>
        public CacheEntryPriority Priority { get; set; } = CacheEntryPriority.Normal;
    }

    /// <summary>
    /// Priorità di mantenimento per le entry della cache
    /// </summary>
    public enum CacheEntryPriority
    {
        /// <summary>
        /// Priorità standard
        /// </summary>
        Normal,

        /// <summary>
        /// Priorità alta (mantenuta più a lungo)
        /// </summary>
        High,

        /// <summary>
        /// Priorità bassa (prima ad essere rimossa)
        /// </summary>
        Low
    }
}