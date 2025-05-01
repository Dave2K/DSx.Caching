
namespace DSx.Caching.Abstractions.Events
{
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
