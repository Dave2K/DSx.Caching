using System;
using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Telemetry
{
    /// <summary>
    /// Definisce le operazioni di telemetria per il monitoraggio della cache.
    /// </summary>
    public interface ICacheTelemetry
    {
        /// <summary>
        /// Registra una richiesta alla cache.
        /// </summary>
        /// <param name="operationName">Nome dell'operazione (es. "Get", "Set").</param>
        /// <param name="duration">Durata dell'operazione.</param>
        /// <param name="success">Indica se l'operazione è riuscita.</param>
        void TrackRequest(string operationName, TimeSpan duration, bool success);

        /// <summary>
        /// Traccia una dipendenza esterna
        /// </summary>
        /// <param name="dependencyType">Tipo di dipendenza (es. Cache, Database)</param>
        /// <param name="target">Target della dipendenza (es. Redis, SQL)</param>
        /// <param name="operationName">Nome dell'operazione eseguita</param>
        /// <param name="success">Indicatore di successo</param>
        /// <param name="startTime">Data/Ora di inizio operazione</param>
        /// <param name="duration">Durata dell'operazione</param>
        void TrackDependency(
            string dependencyType,
            string target,
            string operationName,
            bool success,
            DateTimeOffset startTime,
            TimeSpan duration);

        /// <summary>
        /// Registra un'eccezione non gestita.
        /// </summary>
        /// <param name="exception">Eccezione sollevata.</param>
        /// <param name="context">Contesto aggiuntivo (es. chiave, parametri).</param>
        void TrackException(Exception exception, IDictionary<string, object> context);
    }
}
