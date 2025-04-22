using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Argomenti per l'evento di operazione differita (Funzionalità Enterprise)
    /// </summary>
    public class OperationDeferredEventArgs : EventArgs
    {
        /// <summary>
        /// Chiave dell'operazione differita
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Motivo del differimento
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Inizializza una nuova istanza degli argomenti dell'evento
        /// </summary>
        public OperationDeferredEventArgs(string key, string reason)
        {
            Key = key;
            Reason = reason;
        }
    }
}