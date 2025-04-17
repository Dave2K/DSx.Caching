namespace DSx.Caching.Abstractions.Telemetry
{
    /// <summary>
    /// Fornisce strumenti per il tracciamento delle metriche e delle operazioni della cache
    /// </summary>
    public interface ICacheTelemetry
    {
        /// <summary>
        /// Traccia una richiesta alla cache
        /// </summary>
        /// <param name="operationName">Nome dell'operazione</param>
        /// <param name="duration">Durata dell'operazione</param>
        /// <param name="success">Indicatore di successo</param>
        void TrackRequest(string operationName, TimeSpan duration, bool success);

        /// <summary>
        /// Traccia una dipendenza esterna (es. connessione a Redis)
        /// </summary>
        /// <param name="dependencyType">Tipo di dipendenza</param>
        /// <param name="dependencyName">Nome della dipendenza</param>
        /// <param name="success">Indicatore di successo</param>
        void TrackDependency(string dependencyType, string dependencyName, bool success);

        /// <summary>
        /// Traccia un'eccezione verificatasi durante le operazioni
        /// </summary>
        /// <param name="exception">Eccezione verificatasi</param>
        /// <param name="context">Contesto aggiuntivo</param>
        void TrackException(Exception exception, IDictionary<string, object> context);
    }
}