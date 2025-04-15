using System.Text.Json;
using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Serialization;

namespace DSx.Caching.SharedKernel.Serialization
{
    /// <summary>
    /// Implementazione JSON per la serializzazione nella cache
    /// </summary>
    public class JsonCacheSerializer : ICacheSerializer
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Inizializza una nuova istanza del serializzatore JSON
        /// </summary>
        public JsonCacheSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T value)
        {
            try
            {
                return JsonSerializer.SerializeToUtf8Bytes(value, _options);
            }
            catch (Exception ex)
            {
                throw new CacheSerializationException("Errore serializzazione JSON", ex);
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(data, _options)!;
            }
            catch (Exception ex)
            {
                throw new CacheSerializationException("Errore deserializzazione JSON", ex);
            }
        }
    }
}