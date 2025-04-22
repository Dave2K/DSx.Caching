namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Adattatore per formati di serializzazione avanzati (Funzionalità Enterprise)
    /// </summary>
    public interface ISerializationAdapter
    {
        /// <summary>
        /// Serializza un oggetto nel formato specificato
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="value">Oggetto da serializzare</param>
        /// <returns>Byte array contenente i dati serializzati</returns>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Deserializza un byte array nell'oggetto originale
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="data">Byte array contenente i dati serializzati</param>
        /// <returns>Oggetto deserializzato</returns>
        T Deserialize<T>(byte[] data);
    }
}