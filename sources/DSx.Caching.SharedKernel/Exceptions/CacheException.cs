using System;
using System.Runtime.Serialization;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Eccezione base per errori generici della cache
    /// </summary>
    [Serializable]
    public class CacheException : Exception
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/>
        /// </summary>
        public CacheException() { }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/> con un messaggio specifico
        /// </summary>
        /// <param name="message">Messaggio che descrive l'errore</param>
        public CacheException(string message) : base(ValidateMessage(message)) { }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/> con un messaggio e un'eccezione interna
        /// </summary>
        /// <param name="message">Messaggio che descrive l'errore</param>
        /// <param name="innerException">Eccezione interna che ha causato l'errore</param>
        public CacheException(string message, Exception innerException)
            : base(ValidateMessage(message), innerException) { }

        /// <summary>
        /// Dettagli tecnici dell'errore (solo lettura)
        /// </summary>
        public virtual string TechnicalDetails =>
            $"Cache Failure: {Message}{Environment.NewLine}" +
            $"Exception Type: {GetType().FullName}{Environment.NewLine}" +
            (InnerException != null
                ? $"Inner Exception Type: {InnerException.GetType().FullName}{Environment.NewLine}" +
                  $"Inner Message: {InnerException.Message}{Environment.NewLine}" +
                  $"Inner Stack Trace: {Environment.NewLine}{InnerException.StackTrace}"
                : $"Stack Trace: {Environment.NewLine}{StackTrace}");

        // Validazione del messaggio
        private static string ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Il messaggio deve contenere informazioni significative", nameof(message));

            return message.Trim();
        }

        // Costruttore per la deserializzazione (obsoleto ma necessario per compatibilità)
        protected CacheException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}