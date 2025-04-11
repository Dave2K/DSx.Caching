namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Definisce gli stati possibili di un'operazione sulla cache
    /// </summary>
    public enum CacheOperationStatus
    {
        /// <summary>
        /// Operazione completata con successo
        /// </summary>
        Success,

        /// <summary>
        /// Elemento non trovato
        /// </summary>
        NotFound,

        /// <summary>
        /// Errore di validazione dei parametri
        /// </summary>
        ValidationError,

        /// <summary>
        /// Errore di connessione al sistema di caching
        /// </summary>
        ConnectionError
    }
}