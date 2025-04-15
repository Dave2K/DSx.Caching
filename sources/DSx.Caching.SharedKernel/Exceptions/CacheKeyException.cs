using DSx.Caching.SharedKernel.Enums;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Eccezione sollevata per errori di validazione delle chiavi di cache
    /// </summary>
    /// <param name="message">Messaggio descrittivo</param>
    /// <param name="codiceErrore">Codice univoco errore (es. KEY_001)</param>
    /// <param name="livelloLog">Livello di gravità</param>
    /// <param name="innerException">Eccezione originale</param>
    [Serializable]
    public class CacheKeyException(
        string message,
        string codiceErrore,
        LivelloLog livelloLog,
        Exception innerException) : CacheException(message, innerException)
    {
        /// <summary>
        /// Codice identificativo univoco dell'errore
        /// </summary>
        public string CodiceErrore { get; } = codiceErrore;

        /// <summary>
        /// Livello di log raccomandato per questo tipo di errore
        /// </summary>
        public LivelloLog LivelloLog { get; } = livelloLog;
    }
}