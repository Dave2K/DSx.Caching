namespace DSx.Caching.Abstractions.Telemetry
{
    /// <summary>
    /// Fornisce metriche e tracciamento per le operazioni di cache
    /// </summary>
    public interface ICacheTelemetry
    {
        /// <summary>
        /// Registra una richiesta alla cache
        /// </summary>
        /// <param name="operation">Tipo di operazione</param>
        /// <param name="duration">Durata dell'operazione</param>
        /// <param name="success">Indicatore di successo</param>
        void TrackRequest(string operation, TimeSpan duration, bool success);

        /// <summary>
        /// Registra una dipendenza esterna (es: Redis, SQL)
        /// </summary>
        /// <param name="dependencyType">Tipo di dipendenza</param>
        /// <param name="dependencyName">Nome identificativo</param>
        /// <param name="success">Indicatore di successo</param>
        void TrackDependency(string dependencyType, string dependencyName, bool success);

        /// <summary>
        /// Registra un'eccezione verificatasi
        /// </summary>
        /// <param name="exception">Eccezione verificatasi</param>
        /// <param name="context">Contesto aggiuntivo</param>
        void TrackException(System.Exception exception, IDictionary<string, object> context);
    }
}