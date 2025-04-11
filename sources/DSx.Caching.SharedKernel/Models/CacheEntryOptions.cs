namespace DSx.Caching.SharedKernel.Models
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