using System;
using DSx.Caching.SharedKernel.Enums;

namespace DSx.Caching.SharedKernel.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verificano errori di connessione ai provider di cache
    /// </summary>
    /// <param name="message">Messaggio descrittivo dell'errore</param>
    /// <param name="nomeProvider">Nome del provider di cache coinvolto</param>
    /// <param name="codiceErrore">Codice identificativo univoco dell'errore</param>
    /// <param name="livelloLog">Livello di gravità per il logging</param>
    /// <param name="innerException">Eccezione originale</param>
    [Serializable]
    public class CacheConnectionException(
        string message,
        string nomeProvider,
        string codiceErrore,
        LivelloLog livelloLog,
        Exception innerException) : Exception(message, innerException)
    {
        /// <summary>
        /// Nome del provider di cache utilizzato
        /// </summary>
        public string NomeProvider { get; } = nomeProvider;

        /// <summary>
        /// Codice identificativo dell'errore specifico
        /// </summary>
        public string CodiceErrore { get; } = codiceErrore;

        /// <summary>
        /// Livello di gravità dell'errore
        /// </summary>
        public LivelloLog LivelloLog { get; } = livelloLog;

        /// <summary>
        /// Restituisce una rappresentazione stringa completa dell'errore
        /// </summary>
        public override string ToString() =>
            $"{base.ToString()}\n" +
            $"Provider: {NomeProvider}\n" +
            $"Codice: {CodiceErrore}\n" +
            $"Livello: {LivelloLog}";
    }
}