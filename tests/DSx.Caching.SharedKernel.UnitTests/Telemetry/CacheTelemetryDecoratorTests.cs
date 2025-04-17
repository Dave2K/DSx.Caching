using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Telemetry;
using DSx.Caching.SharedKernel.Telemetry;
using Moq;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Telemetry
{
    /// <summary>
    /// Test suite per verificare il comportamento del decoratore di telemetria
    /// </summary>
    public class CacheTelemetryDecoratorTests
    {
        /// <summary>
        /// Verifica che le dipendenze e le richieste vengano tracciate durante le operazioni GET
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldTrackDependencyAndRequest()
        {
            var innerMock = new Mock<ICacheProvider>();
            var telemetryMock = new Mock<ICacheTelemetry>();

            var decorator = new CacheTelemetryDecorator(innerMock.Object, telemetryMock.Object);
            await decorator.GetAsync<string>("test_key");

            telemetryMock.Verify(t => t.TrackDependency("Cache", "Get", true), Times.Once);
            telemetryMock.Verify(t => t.TrackRequest("Cache.Get", It.IsAny<TimeSpan>(), true), Times.Once);
        }

        /// <summary>
        /// Verifica che le eccezioni vengano tracciate durante le operazioni SET fallite
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldTrackExceptionOnFailure()
        {
            var innerMock = new Mock<ICacheProvider>();
            var telemetryMock = new Mock<ICacheTelemetry>();

            innerMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), null, default))
                .ThrowsAsync(new Exception("Simulated error"));

            var decorator = new CacheTelemetryDecorator(innerMock.Object, telemetryMock.Object);
            await Assert.ThrowsAsync<Exception>(() => decorator.SetAsync("test_key", "value"));

            telemetryMock.Verify(t => t.TrackException(It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
        }
    }
}