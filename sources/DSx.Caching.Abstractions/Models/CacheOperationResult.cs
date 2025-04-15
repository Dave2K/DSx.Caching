using System;

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
        /// <value>
        /// Valore dell'enum <see cref="CacheOperationStatus"/> che indica l'esito
        /// </value>
        public CacheOperationStatus Status { get; init; }

        /// <summary>
        /// Dettagli aggiuntivi sull'esito dell'operazione
        /// </summary>
        /// <value>
        /// Stringa descrittiva con dettagli tecnici o messaggi di errore. 
        /// Null per operazioni riuscite.
        /// </value>
        public string? Details { get; init; }

        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        /// <value>
        /// True se <see cref="Status"/> è <see cref="CacheOperationStatus.Success"/>, 
        /// altrimenti False
        /// </value>
        public bool IsSuccess => Status == CacheOperationStatus.Success;
    }

    /// <summary>
    /// Risultato tipizzato di un'operazione di cache con valore di ritorno
    /// </summary>
    /// <typeparam name="T">Tipo del valore memorizzato in cache</typeparam>
    public class CacheOperationResult<T> : CacheOperationResult
    {
        /// <summary>
        /// Valore recuperato dalla cache
        /// </summary>
        /// <value>
        /// Istanza di tipo <typeparamref name="T"/> se l'operazione è riuscita,
        /// valore di default altrimenti
        /// </value>
        public T? Value { get; init; }
    }

    /// <summary>
    /// Elenco degli stati possibili per un'operazione di cache
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
        /// Errore di validazione degli input (es: formato chiave non valido)
        /// </summary>
        ValidationError,

        /// <summary>
        /// Errore di connessione al backend della cache
        /// </summary>
        ConnectionError,

        /// <summary>
        /// Errore durante la serializzazione/deserializzazione
        /// </summary>
        SerializationError,

        /// <summary>
        /// Operazione annullata prima del completamento
        /// </summary>
        OperationCancelled
    }
}