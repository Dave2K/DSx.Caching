//namespace DSx.Caching.Abstractions.Models
//{
//    /// <summary>
//    /// Opzioni configurabili per una voce della cache
//    /// </summary>
//    public class CacheEntryOptions
//    {
//        /// <summary>
//        /// Durata assoluta della cache
//        /// </summary>
//        public TimeSpan? AbsoluteExpiration { get; set; }

//        /// <summary>
//        /// Durata sliding della cache
//        /// </summary>
//        public TimeSpan? SlidingExpiration { get; set; }

//        /// <summary>
//        /// Dimensione approssimativa della voce in byte
//        /// </summary>
//        public long? Size { get; set; }
//    }
//}

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Opzioni di configurazione per le voci della cache
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Durata di validità della voce nella cache
        /// </summary>
        public TimeSpan? Expiration { get; }

        /// <summary>
        /// Crea una nuova istanza delle opzioni
        /// </summary>
        /// <param name="expiration">Tempo di scadenza della voce</param>
        public CacheEntryOptions(TimeSpan? expiration)
        {
            Expiration = expiration;
        }
    }
}