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
        public CacheOperationStatus Status { get; set; }

        /// <summary>
        /// Dettagli aggiuntivi sull'esito
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        public bool IsSuccess => Status == CacheOperationStatus.Success;
    }

    /// <summary>
    /// Rappresenta un risultato tipizzato di un'operazione sulla cache
    /// </summary>
    /// <typeparam name="T">Tipo del valore restituito</typeparam>
    public class CacheOperationResult<T> : CacheOperationResult
    {
        /// <summary>
        /// Valore restituito dall'operazione
        /// </summary>
        public T? Value { get; set; }
    }
}