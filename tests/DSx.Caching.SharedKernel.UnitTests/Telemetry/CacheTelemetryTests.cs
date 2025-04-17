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
    /// Unit tests for verifying cache telemetry functionality
    /// </summary>
    public class CacheTelemetryTests
    {
        private readonly Mock<ITelemetryChannel> _channelMock = new();
        private readonly TelemetryClient _telemetryClient;

        // Inizializzazione semplificata con collection expression
        private readonly List<ITelemetry> _sentItems = [];

        /// <summary>
        /// Initializes telemetry configuration and client
        /// </summary>
        public CacheTelemetryTests()
        {
            _channelMock.Setup(c => c.Send(It.IsAny<ITelemetry>()))
                .Callback<ITelemetry>(item => _sentItems.Add(item));

            var config = new TelemetryConfiguration
            {
                TelemetryChannel = _channelMock.Object,
                ConnectionString = $"InstrumentationKey={Guid.NewGuid()}"
            };

            _telemetryClient = new TelemetryClient(config);
        }

        /// <summary>
        /// Verifies correct request telemetry transmission
        /// </summary>
        [Fact]
        public void TrackRequest_ShouldSendCorrectTelemetry()
        {
            var telemetry = new AppInsightsTelemetry(_telemetryClient);
            telemetry.TrackRequest("Get", TimeSpan.FromMilliseconds(150), true);

            var request = _sentItems[0] as RequestTelemetry;
            Assert.Equal("Get", request!.Name);
            Assert.True(request.Success);
        }

        /// <summary>
        /// Verifies exception context properties inclusion
        /// </summary>
        [Fact]
        public void TrackException_ShouldIncludeContextProperties()
        {
            var telemetry = new AppInsightsTelemetry(_telemetryClient);
            var context = new Dictionary<string, object> { { "Key", "test" } };

            telemetry.TrackException(new InvalidOperationException("Test"), context);

            var exception = _sentItems[0] as ExceptionTelemetry;
            Assert.Equal("Test", exception!.Exception.Message);
            Assert.Contains("Key", exception.Properties.Keys);
        }
    }
}