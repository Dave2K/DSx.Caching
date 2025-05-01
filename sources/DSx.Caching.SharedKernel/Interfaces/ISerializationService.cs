namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Servizio per la serializzazione/desserializzazione dei dati
    /// </summary>
    public interface ISerializationService
    {
        /// <summary>
        /// Serializza un oggetto
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto</typeparam>
        /// <param name="oggetto">Istanza da serializzare</param>
        /// <returns>Dati serializzati</returns>
        byte[] Serialize<T>(T oggetto);

        /// <summary>
        /// Deserializza dati in un oggetto
        /// </summary>
        /// <typeparam name="T">Tipo target</typeparam>
        /// <param name="dati">Dati da deserializzare</param>
        /// <returns>Oggetto ricostruito</returns>
        T Deserialize<T>(byte[] dati);
    }
}
