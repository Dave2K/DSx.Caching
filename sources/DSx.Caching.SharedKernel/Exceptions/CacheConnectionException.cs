using System;
using System.Runtime.Serialization;
using DSx.Caching.SharedKernel.Enums;

namespace DSx.Caching.SharedKernel.Exceptions
{
    [Serializable]
    public class CacheConnectionException : Exception
    {
        public string NomeProvider { get; }
        public string CodiceErrore { get; }
        public LivelloLog LivelloLog { get; }

        public CacheConnectionException(
            string message,
            string nomeProvider,
            string codiceErrore,
            LivelloLog livelloLog,
            Exception innerException)
            : base(message, innerException)
        {
            NomeProvider = nomeProvider;
            CodiceErrore = codiceErrore;
            LivelloLog = livelloLog;
        }

        protected CacheConnectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            NomeProvider = info.GetString(nameof(NomeProvider))!;
            CodiceErrore = info.GetString(nameof(CodiceErrore))!;
            LivelloLog = (LivelloLog)info.GetValue(nameof(LivelloLog), typeof(LivelloLog))!;
        }

        [Obsolete("Formatter-based serialization is obsolete. Use modern alternatives instead.")]
#pragma warning disable SYSLIB0051
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(NomeProvider), NomeProvider);
            info.AddValue(nameof(CodiceErrore), CodiceErrore);
            info.AddValue(nameof(LivelloLog), LivelloLog);
        }
#pragma warning restore SYSLIB0051
    }
}