namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Opzioni configurabili per una voce della cache
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Durata assoluta della cache
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Durata sliding della cache
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Dimensione approssimativa della voce in byte
        /// </summary>
        public long? Size { get; set; }
    }
}