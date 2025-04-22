using System.Text.Json;
using DSx.Caching.Abstractions.Interfaces;

namespace DSx.Caching.SharedKernel.Serialization
{
    /// <summary>
    /// Adattatore per la serializzazione JSON
    /// </summary>
    public class JsonSerializationAdapter : ISerializationAdapter
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Inizializza un nuovo serializzatore JSON
        /// </summary>
        /// <param name="options">Opzioni di serializzazione</param>
        public JsonSerializationAdapter(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Serializza un oggetto in formato JSON
        /// </summary>
        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _options);
        }

        /// <summary>
        /// Deserializza un byte array in oggetto
        /// </summary>
        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, _options)
                ?? throw new InvalidOperationException("Deserializzazione fallita");
        }
    }
}