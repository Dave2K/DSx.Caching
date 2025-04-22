// SOSTITUIRE TUTTO il contenuto del file
using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.SharedKernel.Enums;
using System;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Eccezione specializzata per errori nelle chiavi di cache
    /// </summary>
    [Serializable]
    public class CacheKeyException : CacheException
    {
        /// <summary>
        /// Codice identificativo dell'errore
        /// </summary>
        public string CodiceErrore { get; }

        /// <summary>
        /// Livello di gravità per il logging
        /// </summary>
        public LivelloLog LivelloLog { get; }

        /// <summary>
        /// Costruttore completo per l'eccezione
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore</param>
        /// <param name="codiceErrore">Codice univoco dell'errore</param>
        /// <param name="livelloLog">Livello di logging appropriato</param>
        /// <param name="innerException">Eccezione originale</param>
        public CacheKeyException(
            string message,
            string codiceErrore,
            LivelloLog livelloLog,
            Exception innerException)
            : base(
                message: message,
                errorCode: codiceErrore,
                innerException: innerException)
        {
            CodiceErrore = codiceErrore;
            LivelloLog = livelloLog;
        }
    }
}