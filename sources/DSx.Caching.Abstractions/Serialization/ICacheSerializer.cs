using DSx.Caching.Abstractions.Exceptions; // Aggiungi questo using

namespace DSx.Caching.Abstractions.Serialization
{
    /// <summary>
    /// Servizio per la serializzazione/deserializzazione degli oggetti
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// Serializza un oggetto in formato binario
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto</typeparam>
        /// <param name="value">Valore da serializzare</param>
        /// <exception cref="CacheSerializationException"> // Riferimento corretto
        /// Sollevata per errori durante la serializzazione
        /// </exception>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Deserializza dati binari in un oggetto
        /// </summary>
        /// <typeparam name="T">Tipo target</typeparam>
        /// <param name="data">Dati binari</param>
        /// <exception cref="CacheSerializationException"> // Riferimento corretto
        /// Sollevata per errori durante la deserializzazione
        /// </exception>
        T Deserialize<T>(byte[] data);
    }
}