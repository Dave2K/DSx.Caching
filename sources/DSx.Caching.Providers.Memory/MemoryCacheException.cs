using System;
using System.Runtime.Serialization;

namespace DSx.Caching.Providers.Memory
{
    [Serializable]
    public class MemoryCacheException : Exception
    {
        public string TechnicalDetails { get; }

        // Costruttore completo
        public MemoryCacheException(
            string message,
            string technicalDetails,
            Exception inner)
            : base(message, inner)
        {
            TechnicalDetails = technicalDetails;
        }

        // Costruttore serializzazione
        protected MemoryCacheException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            TechnicalDetails = info.GetString(nameof(TechnicalDetails))!;
        }

        // Metodo serializzazione
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(TechnicalDetails), TechnicalDetails);
        }
    }
}