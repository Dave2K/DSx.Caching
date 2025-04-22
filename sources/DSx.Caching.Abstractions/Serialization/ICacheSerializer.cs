using DSx.Caching.Abstractions.Exceptions;
using System;

namespace DSx.Caching.Abstractions.Serialization
{
    /// <summary>
    /// Definisce il contratto per la serializzazione/deserializzazione degli oggetti nella cache.
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// Serializza un oggetto in formato binario.
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare.</typeparam>
        /// <param name="value">Istanza da serializzare.</param>
        /// <returns>Byte array contenente i dati serializzati.</returns>
        /// <exception cref="CacheSerializationException">Errore durante la serializzazione.</exception>
        /// <exception cref="ArgumentNullException">Se value è null.</exception>
        /// <example>
        /// <code>
        /// var serializer = new JsonCacheSerializer();
        /// byte[] data = serializer.Serialize(new { Id = 1, Name = "Test" });
        /// </code>
        /// </example>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Deserializza dati binari nell'oggetto originale.
        /// </summary>
        /// <typeparam name="T">Tipo target della deserializzazione.</typeparam>
        /// <param name="data">Dati binari da deserializzare.</param>
        /// <returns>Oggetto deserializzato.</returns>
        /// <exception cref="CacheSerializationException">Errore durante la deserializzazione.</exception>
        /// <exception cref="ArgumentNullException">Se data è null o vuoto.</exception>
        /// <example>
        /// <code>
        /// var obj = serializer.Deserialize&lt;MyClass&gt;(data);
        /// </code>
        /// </example>
        T Deserialize<T>(byte[] data);
    }
}