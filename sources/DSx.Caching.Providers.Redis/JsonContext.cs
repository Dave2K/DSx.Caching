using System.Text.Json;
using System.Text.Json.Serialization;
using DSx.Caching.Abstractions.Exceptions;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Contesto di serializzazione JSON per le eccezioni Redis.
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        WriteIndented = true)]
    [JsonSerializable(typeof(RedisCacheException))]
    internal sealed partial class RedisExceptionJsonContext : JsonSerializerContext
    {
        /// <summary>
        /// Opzioni di serializzazione personalizzate.
        /// </summary>
        public new static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}
