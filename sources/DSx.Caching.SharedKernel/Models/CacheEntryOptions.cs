namespace DSx.Caching.SharedKernel.Models
{
    /// <summary>
    /// Opzioni configurabili per una voce nella cache.
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Durata assoluta della voce nella cache.
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; }

        /// <summary>
        /// Durata rinnovabile della voce nella cache.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; }

        /// <summary>
        /// Crea una nuova istanza di <see cref="CacheEntryOptions"/>.
        /// </summary>
        /// <param name="absoluteExpiration">Durata assoluta.</param>
        /// <param name="slidingExpiration">Durata rinnovabile.</param>
        public CacheEntryOptions(
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            AbsoluteExpiration = absoluteExpiration;
            SlidingExpiration = slidingExpiration;
        }
    }
}