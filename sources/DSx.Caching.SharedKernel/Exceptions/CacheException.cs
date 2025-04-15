using System;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Classe base per tutte le eccezioni relative al sistema di caching
    /// </summary>
    [Serializable]
    public class CacheException : Exception
    {
        /// <summary>
        /// Dettagli tecnici dell'errore per il troubleshooting
        /// </summary>
        public virtual string TechnicalDetails =>
            $"Cache Failure: {Message}{Environment.NewLine}" +
            $"Exception Type: {GetType().FullName}{Environment.NewLine}" +
            (InnerException != null
                ? $"Inner Exception Type: {InnerException.GetType().FullName}{Environment.NewLine}" +
                  $"Inner Message: {InnerException.Message}{Environment.NewLine}" +
                  $"Inner Stack Trace: {Environment.NewLine}{InnerException.StackTrace}"
                : $"Stack Trace: {Environment.NewLine}{StackTrace}");

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/>
        /// </summary>
        public CacheException() { }

        /// <summary>
        /// Inizializza una nuova istanza della classe con un messaggio specifico
        /// </summary>
        /// <param name="message">Messaggio che descrive l'errore</param>
        public CacheException(string message) : base(ValidateMessage(message)) { }

        /// <summary>
        /// Inizializza una nuova istanza della classe con messaggio ed eccezione interna
        /// </summary>
        /// <param name="message">Messaggio descrittivo</param>
        /// <param name="innerException">Eccezione originale</param>
        public CacheException(string message, Exception innerException)
            : base(ValidateMessage(message), innerException) { }

        private static string ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Il messaggio deve contenere informazioni significative", nameof(message));

            return message.Trim();
        }
    }
}