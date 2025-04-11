namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Definisce le opzioni di scadenza per un elemento nella cache.
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Ottiene o imposta la durata assoluta dell'elemento nella cache. 
        /// Dopo questo intervallo di tempo, l'elemento scade indipendentemente dagli accessi.
        /// </summary>
        /// <example>TimeSpan.FromMinutes(30)</example>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Ottiene o imposta la durata "sliding" dell'elemento nella cache.
        /// L'elemento scade se non viene acceduto per questo intervallo di tempo.
        /// </summary>
        /// <example>TimeSpan.FromMinutes(5)</example>
        public TimeSpan? SlidingExpiration { get; set; }
    }
}