using DSx.Caching.Abstractions.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace DSx.Caching.SharedKernel.Telemetry
{
    /// <inheritdoc cref="ICacheTelemetry"/>
    public class AppInsightsTelemetry(TelemetryClient telemetryClient) : ICacheTelemetry
    {
        private readonly TelemetryClient _telemetryClient = telemetryClient;

        /// <inheritdoc/>
        public void TrackRequest(string operationName, TimeSpan duration, bool success)
        {
            var requestTelemetry = new RequestTelemetry
            {
                Name = operationName,
                Duration = duration,
                Success = success,
                ResponseCode = success ? "200" : "500"
            };

            _telemetryClient.TrackRequest(requestTelemetry);
        }

        /// <inheritdoc/>
        public void TrackDependency(string dependencyType, string dependencyName, bool success)
        {
            var dependencyTelemetry = new DependencyTelemetry
            {
                Type = dependencyType,
                Name = dependencyName,
                Success = success,
                Data = "CacheOperation"
            };

            _telemetryClient.TrackDependency(dependencyTelemetry);
        }

        /// <inheritdoc/>
        public void TrackException(Exception exception, IDictionary<string, object> context)
        {
            var telemetry = new ExceptionTelemetry(exception);
            foreach (var item in context)
                telemetry.Properties.Add(item.Key, item.Value.ToString());

            _telemetryClient.TrackException(telemetry);
        }
    }
}