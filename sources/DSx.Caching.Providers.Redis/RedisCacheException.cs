using DSx.Caching.Abstractions.Exceptions;
using System;
using System.Runtime.Serialization;

namespace DSx.Caching.Providers.Redis // Namespace corretto
{
    /// <summary>
    /// Eccezione sollevata per errori specifici di Redis durante le operazioni di caching
    /// </summary>
    [Serializable]
    public sealed class RedisCacheException : CacheException
    {
        /// <summary>
        /// Dettagli tecnici dell'errore (es. comando Redis, configurazione)
        /// </summary>
        public string TechnicalDetails { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe RedisCacheException
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore</param>
        /// <param name="technicalDetails">Dettagli tecnici del contesto dell'errore</param>
        /// <param name="errorCode">Codice identificativo univoco dell'errore</param>
        /// <param name="innerException">Eccezione interna originale</param>
        public RedisCacheException(
            string message,
            string technicalDetails,
            string errorCode = "REDIS_000",
            Exception? innerException = null)
            : base(message, errorCode, innerException)
        {
            TechnicalDetails = technicalDetails;
        }
    }
}
