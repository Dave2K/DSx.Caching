using Microsoft.Extensions.Logging;

namespace DSx.Caching.SharedKernel.Logging
{
    /// <summary>
    /// Servizio di logging centralizzato con funzionalità enterprise
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Registra un messaggio con livello specifico
        /// </summary>
        /// <param name="livello">Livello di gravità</param>
        /// <param name="codiceErrore">Codice identificativo univoco</param>
        /// <param name="messaggio">Testo descrittivo</param>
        /// <param name="datiContesto">Dati aggiuntivi opzionali</param>
        void Log(LogLevel livello, string codiceErrore, string messaggio, IDictionary<string, object>? datiContesto = null);

        /// <summary>
        /// Registra un'operazione di business critica
        /// </summary>
        /// <param name="operazione">Nome dell'operazione</param>
        /// <param name="datiOperazione">Dettagli dell'operazione</param>
        void LogBusinessOperation(string operazione, object datiOperazione);
    }
}