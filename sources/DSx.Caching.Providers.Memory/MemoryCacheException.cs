using DSx.Caching.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using System;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Eccezione specifica per errori nella cache in memoria
    /// </summary>
    public class MemoryCacheException : CacheException
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe MemoryCacheException
        /// </summary>
        /// <param name="message">Messaggio di errore</param>
        /// <param name="innerException">Eccezione interna</param>
        public MemoryCacheException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Inizializza una nuova istanza della classe MemoryCacheException con logger
        /// </summary>
        /// <param name="logger">Istanza del logger</param>
        /// <param name="message">Messaggio di errore</param>
        /// <param name="innerException">Eccezione interna</param>
        public MemoryCacheException(
            ILogger<MemoryCacheException> logger,
            string message,
            Exception innerException)
            : this(message, innerException)
        {
            logger.LogError(
                "Errore MemoryCache - Messaggio: {Message}, Tipo: {ExceptionType}",
                message,
                innerException.GetType().FullName);
        }

        /// <summary>
        /// Ottiene i dettagli tecnici dell'eccezione
        /// </summary>
        public override string TechnicalDetails =>
            base.TechnicalDetails + Environment.NewLine +
            "Provider: MemoryCache" + Environment.NewLine +
            $"Stack Completo: {StackTrace}";
    }
}