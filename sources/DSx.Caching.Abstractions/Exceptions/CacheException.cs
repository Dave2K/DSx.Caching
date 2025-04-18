using System;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Rappresenta un errore generico durante le operazioni sulla cache.
    /// </summary>
    /// <remarks>
    /// Codici di errore supportati:
    /// - <c>CACHE_000</c>: Errore generico non specificato.
    /// - <c>CACHE_101</c>: Errore durante l'acquisizione di un lock distribuito.
    /// - <c>CACHE_SERIALIZATION_ERR</c>: Errore durante la serializzazione/deserializzazione.
    /// </remarks>
    [Serializable]
    public class CacheException : Exception
    {
        /// <summary>
        /// Codice univoco dell'errore.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/>.
        /// </summary>
        public CacheException(string message, string errorCode = "CACHE_000", Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Eccezione sollevata per errori durante l'acquisizione di lock distribuiti.
    /// </summary>
    [Serializable]
    public class CacheDistributedLockException : CacheException
    {
        /// <summary>
        /// Nome della risorsa interessata.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheDistributedLockException"/>.
        /// </summary>
        public CacheDistributedLockException(string resourceName, string message, Exception? inner = null)
            : base(message, "CACHE_101", inner)
        {
            ResourceName = resourceName;
        }
    }
}