using System;

namespace DSx.Caching.Abstractions.Exceptions
{
    /// <summary>
    /// Eccezione sollevata per errori durante la serializzazione/deserializzazione
    /// </summary>
    [Serializable]
    public class CacheSerializationException(string message, Exception? inner = null)
        : CacheException(message, "CACHE_SERIALIZATION_ERR", inner)
    {
    }
}