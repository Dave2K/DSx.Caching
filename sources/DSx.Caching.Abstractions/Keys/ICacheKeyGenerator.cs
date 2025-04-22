namespace DSx.Caching.Abstractions.Keys
{
    /// <summary>
    /// Servizio per la generazione e normalizzazione di chiavi di cache.
    /// </summary>
    /// <remarks>
    /// Implementazioni tipiche includono:
    /// - Generazione di chiavi basate su namespace
    /// - Hashing di parametri complessi
    /// - Normalizzazione di caratteri speciali
    /// </remarks>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// Genera una chiave di cache standardizzata.
        /// </summary>
        /// <param name="baseKey">Identificatore base (es. "Utente").</param>
        /// <param name="parameters">Parametri aggiuntivi (es. ID utente).</param>
        /// <returns>
        /// Chiave normalizzata (es. "Utente_123" o "Utente_ABC123").
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Sollevata se baseKey è null o vuoto.
        /// </exception>
        /// <example>
        /// <code>
        /// var key = generator.GenerateKey("Prodotto", productId, categoryId);
        /// </code>
        /// </example>
        string GenerateKey(string baseKey, params object[] parameters);

        /// <summary>
        /// Normalizza una chiave per l'uso nella cache.
        /// </summary>
        /// <param name="rawKey">Chiave non processata.</param>
        /// <returns>
        /// Chiave valida per la cache (senza caratteri speciali).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Sollevata se rawKey non può essere normalizzata.
        /// </exception>
        string NormalizeKey(string rawKey);
    }
}