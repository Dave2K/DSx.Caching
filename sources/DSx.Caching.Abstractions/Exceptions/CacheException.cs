using System;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Eccezione base per tutti gli errori relativi alle operazioni di caching
    /// </summary>
    /// <remarks>
    /// Fornisce un codice errore standardizzato per una migliore gestione degli errori
    /// </remarks>
    [Serializable]
    public class CacheException(string message, string errorCode = "CACHE_000", Exception? innerException = null)
        : Exception(message, innerException)
    {
        /// <summary>
        /// Codice univoco identificativo dell'errore
        /// </summary>
        /// <value>
        /// Stringa nel formato CACHE_XXX dove XXX è un numero univoco
        /// </value>
        public string ErrorCode { get; } = errorCode;
    }

    /// <summary>
    /// Eccezione specifica per errori nei lock distribuiti
    /// </summary>
    [Serializable]
    public class CacheDistributedLockException(string resourceName, string message, Exception? inner = null)
        : CacheException(message, "CACHE_101", inner)
    {
        /// <summary>
        /// Nome della risorsa su cui si è verificato l'errore di lock
        /// </summary>
        public string ResourceName { get; } = resourceName;
    }
}