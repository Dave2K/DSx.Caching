namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Rappresenta il risultato generico di un'operazione sulla cache
    /// </summary>
    public class CacheOperationResult
    {
        /// <summary>
        /// Stato dell'operazione
        /// </summary>
        public CacheOperationStatus Status { get; }

        /// <summary>
        /// Dettagli aggiuntivi sull'esito dell'operazione
        /// </summary>
        public string? Details { get; }

        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        public bool IsSuccess => Status == CacheOperationStatus.Success;

        /// <summary>
        /// Crea un nuovo risultato di operazione
        /// </summary>
        /// <param name="status">Stato dell'operazione</param>
        /// <param name="details">Dettagli aggiuntivi</param>
        public CacheOperationResult(
            CacheOperationStatus status,
            string? details = null)
        {
            Status = status;
            Details = details;
        }
    }

    /// <summary>
    /// Risultato di un'operazione sulla cache con valore restituito
    /// </summary>
    /// <typeparam name="T">Tipo del valore restituito</typeparam>
    public class CacheOperationResult<T> : CacheOperationResult
    {
        /// <summary>
        /// Valore restituito dall'operazione
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Crea un nuovo risultato con valore
        /// </summary>
        /// <param name="value">Valore restituito</param>
        /// <param name="status">Stato dell'operazione</param>
        /// <param name="details">Dettagli aggiuntivi</param>
        public CacheOperationResult(
            T? value,
            CacheOperationStatus status,
            string? details = null)
            : base(status, details)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Enumerazione degli stati possibili per un'operazione sulla cache
    /// </summary>
    public enum CacheOperationStatus
    {
        /// <summary>Operazione riuscita</summary>
        Success,
        /// <summary>Elemento non trovato</summary>
        NotFound,
        /// <summary>Errore di connessione</summary>
        ConnectionError,
        /// <summary>Errore di validazione</summary>
        ValidationError
    }
}
