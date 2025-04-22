using System;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Gestore delle regole TTL dinamiche (Funzionalità Enterprise)
    /// </summary>
    public interface ITTLManager
    {
        /// <summary>
        /// Configura una regola di scadenza per un pattern di chiavi
        /// </summary>
        /// <param name="keyPattern">Pattern delle chiavi (es: "prodotti_*")</param>
        /// <param name="ttl">Tempo di vita della voce</param>
        void ConfigureRule(string keyPattern, TimeSpan ttl);

        /// <summary>
        /// Ottiene il TTL configurato per una specifica chiave
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <returns>TTL applicabile o null</returns>
        TimeSpan? GetTTLForEntry(string key);
    }
}