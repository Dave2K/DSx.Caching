// File: DSx.Caching.SharedKernel/Telemetry/AppInsightsTelemetry.cs
using DSx.Caching.Abstractions.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace DSx.Caching.SharedKernel.Telemetry
{
    /// <summary>
    /// Fornisce un'implementazione di <see cref="ICacheTelemetry"/> che utilizza Application Insights per il tracciamento.
    /// </summary>
    public class AppInsightsTelemetry : ICacheTelemetry
    {
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="AppInsightsTelemetry"/>
        /// </summary>
        /// <param name="telemetryClient">Client Application Insights per l'invio della telemetria</param>
        public AppInsightsTelemetry(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <summary>
        /// Registra una richiesta nel sistema di telemetria.
        /// </summary>
        /// <param name="operationName">Nome dell'operazione.</param>
        /// <param name="duration">Durata dell'operazione.</param>
        /// <param name="success">Indica se l'operazione è riuscita.</param>
        public void TrackRequest(
            string operationName,
            TimeSpan duration,
            bool success)
        {
            var request = new RequestTelemetry
            {
                Name = $"Cache_{operationName}",
                Success = success,
                Duration = duration,
                Timestamp = DateTimeOffset.UtcNow
            };

            _telemetryClient.TrackRequest(request);
        }

        /// <summary>
        /// Registra una dipendenza nel sistema di telemetria.
        /// </summary>
        /// <param name="dependencyType">Tipo di dipendenza.</param>
        /// <param name="target">Target della dipendenza.</param>
        /// <param name="operationName">Nome dell'operazione.</param>
        /// <param name="success">Indica se l'operazione è riuscita.</param>
        /// <param name="startTime">Ora di inizio.</param>
        /// <param name="duration">Durata dell'operazione.</param>
        public void TrackDependency(
            string dependencyType,
            string target,
            string operationName,
            bool success,
            DateTimeOffset startTime,
            TimeSpan duration)
        {
            var dependency = new DependencyTelemetry
            {
                Type = dependencyType,
                Target = target,
                Name = operationName,
                Success = success,
                Timestamp = startTime,
                Duration = duration
            };

            _telemetryClient.TrackDependency(dependency);
        }

        /// <summary>
        /// Registra un'eccezione nel sistema di telemetria.
        /// </summary>
        /// <param name="exception">Eccezione da registrare.</param>
        /// <param name="context">Contesto aggiuntivo.</param>
        public void TrackException(
            Exception exception,
            IDictionary<string, object> context)
        {
            var exceptionTelemetry = new ExceptionTelemetry(exception)
            {
                Timestamp = DateTimeOffset.UtcNow
            };

            foreach (var item in context)
            {
                exceptionTelemetry.Properties[item.Key] = item.Value?.ToString();
            }

            _telemetryClient.TrackException(exceptionTelemetry);
        }

        /// <summary>
        /// Invia tutti i dati di telemetria accumulati.
        /// </summary>
        public void Flush()
        {
            _telemetryClient.Flush();
        }

        /// <summary>
        /// Rilascia le risorse utilizzate dall'oggetto.
        /// </summary>
        public void Dispose()
        {
            _telemetryClient.Flush();
            // Rimossa chiamata superfluà a GC.SuppressFinalize
        }
    }
}