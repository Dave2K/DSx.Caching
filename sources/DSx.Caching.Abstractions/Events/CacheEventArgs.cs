using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Rappresenta gli argomenti degli eventi relativi alle operazioni sulla cache
    /// </summary>
    public class CacheEventArgs : EventArgs
    {
        /// <summary>
        /// La chiave coinvolta nell'operazione
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Il tipo di operazione eseguita
        /// </summary>
        public CacheOperationType OperationType { get; }

        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Crea una nuova istanza di CacheEventArgs
        /// </summary>
        /// <param name="key">La chiave coinvolta</param>
        /// <param name="operationType">Tipo di operazione</param>
        /// <param name="success">Esito dell'operazione</param>
        /// <exception cref="ArgumentNullException">Se la chiave è null o vuota</exception>
        public CacheEventArgs(string key, CacheOperationType operationType, bool success = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Key = key;
            OperationType = operationType;
            Success = success;
        }
    }

    /// <summary>
    /// Tipi di operazioni supportate sulla cache
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary>Operazione di lettura</summary>
        Get,
        /// <summary>Operazione di scrittura</summary>
        Set,
        /// <summary>Operazione di rimozione</summary>
        Remove,
        /// <summary>Svuotamento completo</summary>
        ClearAll,
        /// <summary>Verifica esistenza</summary>
        Exists,
        /// <summary>Recupero metadati</summary>
        GetMetadata
    }
}