using System;
using System.Text.Json.Serialization;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Eccezione specifica per errori nella memoria cache
    /// </summary>
    [JsonConverter(typeof(MemoryCacheExceptionConverter))]
    public class MemoryCacheException(string message, string technicalDetails, Exception? innerException = null)
        : Exception(message, innerException)
    {
        /// <summary>
        /// Dettagli tecnici dell'errore
        /// </summary>
        public string TechnicalDetails { get; } = technicalDetails;
    }
}