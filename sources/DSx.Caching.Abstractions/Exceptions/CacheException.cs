using System;
using System.Runtime.Serialization;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Rappresenta un errore generico durante le operazioni sulla cache.
    /// </summary>
    /// <remarks>
    /// Include un codice di errore univoco per identificare il tipo di problema.
    /// </remarks>
    [Serializable]
    public class CacheException : Exception
    {
        /// <summary>
        /// Codice univoco dell'errore.
        /// </summary>
        /// <value>
        /// Stringa che identifica la categoria dell'errore (es. "CACHE_000").
        /// </value>
        public string ErrorCode { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/>.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore.</param>
        /// <param name="errorCode">Codice univoco dell'errore (default: "CACHE_000").</param>
        /// <param name="innerException">Eccezione interna correlata.</param>
        public CacheException(
            string message,
            string errorCode = "CACHE_000",
            Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Costruttore per la deserializzazione (obsoleto in .NET 9).
        /// </summary>
        /// <param name="info">Dati serializzati.</param>
        /// <param name="context">Contesto di serializzazione.</param>
        [Obsolete("Questo costruttore è obsoleto in .NET 9+")]
        protected CacheException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            ErrorCode = info.GetString(nameof(ErrorCode)) ?? "CACHE_000";
        }
    }
}