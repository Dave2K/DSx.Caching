using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using DSx.Caching.Abstractions.Exceptions;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Eccezione specifica per errori nelle operazioni del provider Redis
    /// </summary>
    [Serializable]
    public class RedisCacheException : CacheException
    {
        /// <summary>
        /// Dettagli tecnici dell'errore (configurazione, comando Redis, ecc.)
        /// </summary>
        public string TechnicalDetails { get; }

        /// <summary>
        /// Crea una nuova istanza dell'eccezione
        /// </summary>
        /// <param name="message">Descrizione human-readable dell'errore</param>
        /// <param name="technicalDetails">Dettagli tecnici per il debugging</param>
        /// <param name="errorCode">Codice univoco dell'errore (formato: REDIS_XXX)</param>
        /// <param name="innerException">Eccezione originale (se presente)</param>
        public RedisCacheException(
            string message,
            string technicalDetails,
            string errorCode = "REDIS_000",
            Exception? innerException = null)
            : base(message, errorCode, innerException)
        {
            TechnicalDetails = technicalDetails;
        }

        /// <summary>
        /// Costruttore per deserializzazione (obsoleto ma necessario)
        /// </summary>
        [Obsolete("Obsoleto in .NET 9, mantenuto per compatibilità")]
        protected RedisCacheException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            TechnicalDetails = info.GetString(nameof(TechnicalDetails)) ?? string.Empty;
        }

        /// <summary>
        /// Popola i dati per la serializzazione
        /// </summary>
        [Obsolete("Obsoleto in .NET 9, mantenuto per compatibilità")]
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(TechnicalDetails), TechnicalDetails);
        }
    }
}
