using System;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Rappresenta un'eccezione generata durante le operazioni di caching.
    /// </summary>
    public class CacheException : Exception
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/> con un messaggio specifico.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore. Non può essere vuoto o contenere solo spazi bianchi.</param>
        /// <exception cref="ArgumentException">Generato se <paramref name="message"/> è vuoto o contiene solo spazi bianchi.</exception>
        public CacheException(string message)
            : base(ValidateMessage(message))
        {
        }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheException"/> con un messaggio e un'eccezione interna.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore.</param>
        /// <param name="innerException">Eccezione originale che ha causato l'errore corrente.</param>
        /// <exception cref="ArgumentException">Generato se <paramref name="message"/> è vuoto o contiene solo spazi bianchi.</exception>
        public CacheException(string message, Exception innerException)
            : base(ValidateMessage(message), innerException)
        {
        }

        /// <summary>
        /// Ottiene dettagli tecnici formattati per il logging, inclusi tipo dell'eccezione, messaggio e stack trace.
        /// </summary>
        /// <value>
        /// Stringa formattata contenente:
        /// - Messaggio dell'eccezione
        /// - Tipo dell'eccezione corrente
        /// - Tipo e messaggio dell'eccezione interna (se presente)
        /// - Stack trace dell'eccezione interna (se presente)
        /// </value>
        public virtual string TechnicalDetails =>
            "Cache Failure: " + Message + Environment.NewLine +
            "Exception Type: " + GetType().FullName + Environment.NewLine +
            (InnerException != null
                ? "Inner Exception Type: " + InnerException.GetType().FullName + Environment.NewLine +
                  "Inner Message: " + InnerException.Message + Environment.NewLine +
                  "Inner Stack Trace: " + Environment.NewLine + InnerException.StackTrace
                : "Stack Trace: " + Environment.NewLine + StackTrace);

        private static string ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Il messaggio deve contenere informazioni significative", nameof(message));

            return message.Trim();
        }
    }
}