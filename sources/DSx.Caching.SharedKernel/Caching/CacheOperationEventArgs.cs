using System;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Classe che contiene gli argomenti degli eventi delle operazioni sulla cache
    /// </summary>
    public class CacheOperationEventArgs : EventArgs
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe CacheOperationEventArgs
        /// </summary>
        /// <param name="operationType">Tipo di operazione eseguita</param>
        /// <param name="key">Chiave interessata dall'operazione</param>
        public CacheOperationEventArgs(string operationType, string key)
        {
            OperationType = operationType;
            Key = key;
        }

        /// <summary>
        /// Ottiene il tipo di operazione eseguita
        /// </summary>
        public string OperationType { get; }

        /// <summary>
        /// Ottiene la chiave interessata dall'operazione
        /// </summary>
        public string Key { get; }
    }
}
