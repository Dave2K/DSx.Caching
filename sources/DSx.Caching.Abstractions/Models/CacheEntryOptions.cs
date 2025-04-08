namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Opzioni di configurazione per una voce della cache
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Durata assoluta della voce nella cache
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Durata sliding della voce nella cache
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }
    }
}