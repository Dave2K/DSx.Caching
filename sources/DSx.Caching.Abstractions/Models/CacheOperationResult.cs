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
        /// Dettagli aggiuntivi sull'operazione
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        public bool IsSuccess => Status == CacheOperationStatus.Success;
    }

    /// <summary>
    /// Risultato di un'operazione di cache con valore di ritorno
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
    /// Enumerazione degli stati possibili di un'operazione di cache
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
        /// Errore di validazione
        /// </summary>
        ValidationError,

        /// <summary>
        /// Errore di connessione
        /// </summary>
        ConnectionError
    }
}