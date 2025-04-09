namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Risultato generico di un'operazione di cache
    /// </summary>
    public class CacheOperationResult
    {
        /// <summary>
        /// Stato dell'operazione
        /// </summary>
        public CacheOperationStatus Status { get; set; }

        /// <summary>
        /// Dettagli aggiuntivi sull'esito
        /// </summary>
        public string? Details { get; set; }
    }

    /// <summary>
    /// Risultato tipizzato di un'operazione di cache
    /// </summary>
    /// <typeparam name="T">Tipo del valore restituito</typeparam>
    public class CacheOperationResult<T> : CacheOperationResult
    {
        /// <summary>
        /// Valore recuperato dalla cache
        /// </summary>
        public T? Value { get; set; }
    }

    /// <summary>
    /// Enumerazione degli stati possibili per un'operazione
    /// </summary>
    public enum CacheOperationStatus
    {
        /// <summary>Operazione riuscita</summary>
        Success,
        /// <summary>Elemento non trovato</summary>
        NotFound,
        /// <summary>Errore di validazione</summary>
        ValidationError
    }
}