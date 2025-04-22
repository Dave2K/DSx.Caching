using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Rappresenta gli argomenti degli eventi relativi alle operazioni sulla cache.
    /// </summary>
    /// <remarks>
    /// Utilizzato per notificare:
    /// - Operazioni completate (get/set/remove)
    /// - Errori durante le operazioni
    /// - Statistiche di utilizzo
    /// </remarks>
    public class CacheEventArgs : EventArgs
    {
        /// <summary>
        /// La chiave coinvolta nell'operazione.
        /// </summary>
        /// <value>
        /// Identificatore univoco dell'elemento nella cache.
        /// </value>
        public string Key { get; }

        /// <summary>
        /// Il tipo di operazione eseguita.
        /// </summary>
        public CacheOperationType OperationType { get; }

        /// <summary>
        /// Indica se l'operazione è riuscita.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Dettagli aggiuntivi sull'operazione.
        /// </summary>
        public string? AdditionalInfo { get; }

        /// <summary>
        /// Inizializza una nuova istanza della classe CacheEventArgs.
        /// </summary>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="operationType">Tipo di operazione.</param>
        /// <param name="success">Esito dell'operazione.</param>
        /// <param name="additionalInfo">Informazioni aggiuntive (opzionale).</param>
        /// <exception cref="ArgumentNullException">Se la chiave è null o vuota.</exception>
        public CacheEventArgs(
            string key,
            CacheOperationType operationType,
            bool success = false,
            string? additionalInfo = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            OperationType = operationType;
            Success = success;
            AdditionalInfo = additionalInfo;
        }
    }

    /// <summary>
    /// Tipi di operazioni supportate sulla cache.
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary>Operazione di lettura.</summary>
        Get,

        /// <summary>Operazione di scrittura.</summary>
        Set,

        /// <summary>Operazione di rimozione.</summary>
        Remove,

        /// <summary>Svuotamento completo della cache.</summary>
        ClearAll,

        /// <summary>Verifica esistenza elemento.</summary>
        Exists,

        /// <summary>Recupero metadati elemento.</summary>
        GetMetadata,

        /// <summary>Aggiornamento valori esistenti.</summary>
        Update
    }
}