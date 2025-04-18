// DSx.Caching.Abstractions/Events/CacheEventArgs.cs
using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Argomenti per gli eventi delle operazioni sulla cache
    /// </summary>
    public class CacheEventArgs : EventArgs
    {
        /// <summary>
        /// Chiave interessata dall'operazione
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Tipo di operazione eseguita
        /// </summary>
        public CacheOperationType OperationType { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe CacheEventArgs
        /// </summary>
        /// <param name="key">Chiave dell'operazione</param>
        /// <param name="operationType">Tipo di operazione</param>
        public CacheEventArgs(string key, CacheOperationType operationType)
        {
            Key = key;
            OperationType = operationType;
        }
    }

    /// <summary>
    /// Tipologia di operazioni sulla cache
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary>Operazione di lettura</summary>
        Get,
        /// <summary>Operazione di scrittura</summary>
        Set,
        /// <summary>Operazione di rimozione</summary>
        Remove,
        /// <summary>Operazione di svuotamento totale</summary>
        ClearAll
    }
}