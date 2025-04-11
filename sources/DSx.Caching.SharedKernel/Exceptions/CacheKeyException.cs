using DSx.Caching.SharedKernel.Enums;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Eccezione specifica per errori nelle chiavi della cache
    /// </summary>
    [Serializable]
    public class CacheKeyException : CacheException
    {
        public string CodiceErrore { get; }
        public LivelloLog LivelloLog { get; }

        public CacheKeyException(
            string message,
            string codiceErrore,
            LivelloLog livelloLog,
            Exception innerException)
            : base(message, innerException) // Chiamata al costruttore base corretto
        {
            CodiceErrore = codiceErrore;
            LivelloLog = livelloLog;
        }
    }
}