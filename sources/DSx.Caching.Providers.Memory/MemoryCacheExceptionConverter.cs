using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DSx.Caching.Providers.Memory
{
    /// <summary>
    /// Fornisce la conversione personalizzata per la serializzazione e deserializzazione
    /// delle eccezioni <see cref="MemoryCacheException"/> utilizzando System.Text.Json
    /// </summary>
    public class MemoryCacheExceptionConverter : JsonConverter<MemoryCacheException>
    {
        /// <summary>
        /// Deserializza un'eccezione <see cref="MemoryCacheException"/> da JSON
        /// </summary>
        /// <param name="reader">Lettore JSON di input</param>
        /// <param name="typeToConvert">Tipo di destinazione della conversione</param>
        /// <param name="options">Opzioni di serializzazione</param>
        /// <returns>Istanza deserializzata di <see cref="MemoryCacheException"/></returns>
        /// <exception cref="JsonException">Errore durante la deserializzazione</exception>
        public override MemoryCacheException Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var message = root.GetProperty("Message").GetString()!;
            var technicalDetails = root.GetProperty("TechnicalDetails").GetString()!;

            Exception? innerException = null;
            if (root.TryGetProperty("InnerException", out var innerExceptionElement) &&
                innerExceptionElement.ValueKind != JsonValueKind.Null)
            {
                var innerType = Type.GetType(innerExceptionElement.GetProperty("Type").GetString()!) ?? typeof(Exception);
                var innerMessage = innerExceptionElement.GetProperty("Message").GetString();
                innerException = (Exception)Activator.CreateInstance(innerType, innerMessage)!;
            }

            return new MemoryCacheException(message, technicalDetails, innerException);
        }

        /// <summary>
        /// Serializza un'eccezione <see cref="MemoryCacheException"/> in JSON
        /// </summary>
        /// <param name="writer">Scrittore JSON di output</param>
        /// <param name="value">Istanza da serializzare</param>
        /// <param name="options">Opzioni di serializzazione</param>
        public override void Write(
            Utf8JsonWriter writer,
            MemoryCacheException value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Message", value.Message);
            writer.WriteString("TechnicalDetails", value.TechnicalDetails);

            writer.WritePropertyName("InnerException");
            if (value.InnerException != null)
            {
                writer.WriteStartObject();
                writer.WriteString("Type", value.InnerException.GetType().AssemblyQualifiedName);
                writer.WriteString("Message", value.InnerException.Message);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WriteEndObject();
        }
    }
}