using System;

namespace DSx.Caching
{
    /// <summary>
    /// Eccezione generata quando si tenta di accedere a un provider non configurato
    /// </summary>
    public class ProviderNotConfiguredException : Exception
    {
        /// <summary>
        /// Inizializza una nuova istanza dell'eccezione
        /// </summary>
        public ProviderNotConfiguredException() { }

        /// <summary>
        /// Inizializza una nuova istanza con un messaggio specifico
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore</param>
        public ProviderNotConfiguredException(string message) : base(message) { }

        /// <summary>
        /// Inizializza una nuova istanza con un messaggio e un'eccezione interna
        /// </summary>
        /// <param name="message">Messaggio descrittivo</param>
        /// <param name="innerException">Eccezione originale</param>
        public ProviderNotConfiguredException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
