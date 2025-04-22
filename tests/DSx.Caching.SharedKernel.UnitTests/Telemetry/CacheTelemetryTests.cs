// File modificato: DSx.Caching.SharedKernel.UnitTests/Telemetry/CacheTelemetryTests.cs
using DSx.Caching.SharedKernel.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Telemetry
{
    /// <summary>
    /// Contiene i test per il tracciamento della telemetria della cache
    /// </summary>
    public class CacheTelemetryTests
    {
        private readonly Mock<ITelemetryChannel> _channelMock = new();
        private readonly TelemetryClient _telemetryClient;
        private readonly List<ITelemetry> _sentItems = new();

        /// <summary>
        /// Inizializza la configurazione di telemetria per i test
        /// </summary>
        public CacheTelemetryTests()
        {
            _channelMock.Setup(c => c.Send(It.IsAny<ITelemetry>()))
                .Callback<ITelemetry>(item => _sentItems.Add(item));

            var config = new TelemetryConfiguration
            {
                TelemetryChannel = _channelMock.Object,
                ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000"
            };

            _telemetryClient = new TelemetryClient(config);
        }

        /// <summary>
        /// Verifica che le richieste vengano tracciate correttamente
        /// </summary>
        [Fact]
        public void TrackRequest_ShouldSendCorrectTelemetry()
        {
            // Arrange
            var telemetry = new AppInsightsTelemetry(_telemetryClient); // <-- Modifica chiave qui

            // Act
            telemetry.TrackRequest("Get", TimeSpan.FromMilliseconds(150), true);

            // Assert
            Assert.Single(_sentItems);
            var request = _sentItems[0] as RequestTelemetry;
            Assert.Equal("Cache_Get", request!.Name);
            Assert.True(request.Success);
        }

        /// <summary>
        /// Verifica che le eccezioni includano le proprietà del contesto
        /// </summary>
        [Fact]
        public void TrackException_ShouldIncludeContextProperties()
        {
            // Arrange
            var telemetry = new AppInsightsTelemetry(_telemetryClient); // <-- Modifica chiave qui
            var context = new Dictionary<string, object> { { "Key", "test" } };

            // Act
            telemetry.TrackException(new InvalidOperationException("Test"), context);

            // Assert
            Assert.Single(_sentItems);
            var exception = _sentItems[0] as ExceptionTelemetry;
            Assert.Contains("Key", exception!.Properties.Keys);
        }
    }
}