using DSx.Caching.Abstractions.Exceptions;
using System;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Eccezione specifica per errori nella cache Redis
    /// </summary>
    public class RedisCacheException : CacheException
    {
        /// <summary>
        /// Inizializza una nuova istanza con messaggio predefinito
        /// </summary>
        public RedisCacheException() : base("Errore nella cache Redis") { }

        /// <summary>
        /// Inizializza una nuova istanza con messaggio personalizzato
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore</param>
        public RedisCacheException(string message) : base(message) { }

        /// <summary>
        /// Inizializza una nuova istanza con messaggio ed eccezione interna
        /// </summary>
        /// <param name="message">Messaggio descrittivo</param>
        /// <param name="innerException">Eccezione che ha causato l'errore</param>
        public RedisCacheException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Ottiene i dettagli tecnici dell'errore
        /// </summary>
        public override string TechnicalDetails =>
            $"ERRORE REDIS: {Message}{Environment.NewLine}" +
            $"Tipo: {GetType().Name}{Environment.NewLine}" +
            $"Stack Trace:{Environment.NewLine}{StackTrace}" +
            (InnerException != null ? $"{Environment.NewLine}ECCEZIONE INTERNA: {InnerException.Message}" : "");
    }
}