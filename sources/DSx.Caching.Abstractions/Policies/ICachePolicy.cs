using DSx.Caching.Abstractions.Models;

namespace DSx.Caching.Abstractions.Policies
{
    /// <summary>
    /// Definisce il contratto per le politiche di gestione delle voci nella cache
    /// </summary>
    public interface ICachePolicy
    {
        /// <summary>
        /// Determina se una voce della cache dovrebbe essere rimossa
        /// </summary>
        /// <param name="entry">Descrittore della voce della cache</param>
        /// <returns>True se la voce dovrebbe essere rimossa, altrimenti False</returns>
        bool ShouldEvict(CacheEntryDescriptor entry);

        /// <summary>
        /// Determina se una voce della cache dovrebbe essere aggiornata
        /// </summary>
        /// <param name="entry">Descrittore della voce della cache</param>
        /// <returns>True se la voce richiede un aggiornamento, altrimenti False</returns>
        bool ShouldRefresh(CacheEntryDescriptor entry);

        /// <summary>
        /// Calcola la priorità di ritenzione della voce nella cache
        /// </summary>
        /// <param name="entry">Descrittore della voce della cache</param>
        /// <returns>Valore numerico che indica la priorità (maggiore = più importante)</returns>
        int CalculateRetentionPriority(CacheEntryDescriptor entry);
    }
}
