namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Risultato generico di un'operazione sulla cache
    /// </summary>
    public class CacheOperationResult
    {
        /// <summary>
        /// Stato dell'operazione
        /// </summary>
        public CacheOperationStatus Status { get; set; }

        /// <summary>
        /// Dettagli aggiuntivi sull'esito dell'operazione
        /// </summary>
        public string? Details { get; set; }
    }

    /// <summary>
    /// Risultato tipizzato di un'operazione sulla cache
    /// </summary>
    /// <typeparam name="T">Tipo del valore restituito</typeparam>
    public class CacheOperationResult<T> : CacheOperationResult
    {
        /// <summary>
        /// Valore restituito dall'operazione
        /// </summary>
        public T? Value { get; set; }
    }

    /// <summary>
    /// Stati possibili per un'operazione sulla cache
    /// </summary>
    public enum CacheOperationStatus
    {
        /// <summary>
        /// Operazione completata con successo
        /// </summary>
        Success,

        /// <summary>
        /// Elemento non trovato nella cache
        /// </summary>
        NotFound,

        /// <summary>
        /// Errore di validazione dei dati
        /// </summary>
        ValidationError
    }
}