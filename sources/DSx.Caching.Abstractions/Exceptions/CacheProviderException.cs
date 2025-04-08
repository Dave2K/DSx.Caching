using System;
using System.Runtime.Serialization;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Eccezione base per gli errori dei provider di cache
    /// </summary>
    [Serializable]
    public class CacheProviderException : Exception
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheProviderException"/>
        /// </summary>
        public CacheProviderException() { }

        /// <summary>
        /// Inizializza una nuova istanza con un messaggio specifico
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore</param>
        public CacheProviderException(string message) : base(message) { }

        /// <summary>
        /// Inizializza una nuova istanza con messaggio ed eccezione interna
        /// </summary>
        /// <param name="message">Messaggio descrittivo</param>
        /// <param name="innerException">Eccezione originale</param>
        public CacheProviderException(string message, Exception innerException)
            : base(message, innerException) { }

        // Soluzione per .NET 9.0+ (rimozione serializzazione legacy)
#if !NET8_0_OR_GREATER
        [Obsolete("Obsoleto per .NET 9.0+", DiagnosticId = "SYSLIB0051")]
        protected CacheProviderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
#endif
    }
}