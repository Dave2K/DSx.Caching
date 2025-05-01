using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Telemetry;
using DSx.Caching.SharedKernel.Telemetry;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Telemetry
{
    /// <summary>
    /// Contiene i test per la classe <see cref="CacheTelemetryDecorator"/>
    /// </summary>
    public class CacheTelemetryDecoratorTests
    {
        private readonly Mock<ICacheProvider> _providerMock;
        private readonly Mock<ICacheTelemetry> _telemetryMock;
        private readonly CacheTelemetryDecorator _decorator;

        /// <summary>
        /// Costruttore che inizializza i mock e il decorator
        /// </summary>
        public CacheTelemetryDecoratorTests()
        {
            _providerMock = new Mock<ICacheProvider>();
            _telemetryMock = new Mock<ICacheTelemetry>();
            _decorator = new CacheTelemetryDecorator(
                _providerMock.Object,
                _telemetryMock.Object,
                Mock.Of<ILogger<CacheTelemetryDecorator>>()
            );
        }

        /// <summary>
        /// Verifica che le operazioni annullate vengano tracciate correttamente
        /// </summary>
        [Fact]
        public async Task Operazioni_DovrebberoTracciareCancellazioni()
        {
            // Configurazione
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _providerMock
                .Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheOperationResult { Status = CacheOperationStatus.OperationCancelled });

            // Esecuzione
            var result = await _decorator.ExistsAsync("test_key", cts.Token);

            // Verifica
            _telemetryMock.Verify(t => t.TrackDependency(
                "Cache",
                "Exists",
                "test_key",
                false,
                It.IsAny<DateTimeOffset>(),
                It.IsAny<TimeSpan>()),
                Times.Once);

            result.Status.Should().Be(CacheOperationStatus.OperationCancelled);
        }

        /// <summary>
        /// Verifica che il tracciamento delle operazioni Get avvenga correttamente
        /// </summary>
        [Fact]
        public async Task GetAsync_DovrebbeTracciareOperazioneRiuscita()
        {
            // Configurazione
            const string testKey = "chiave_valida";
            var expectedResult = new CacheOperationResult<string>
            {
                Status = CacheOperationStatus.Success,
                Value = "valore_test"
            };

            _providerMock
                .Setup(x => x.GetAsync<string>(testKey, It.IsAny<CacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Esecuzione
            var result = await _decorator.GetAsync<string>(testKey);

            // Verifica
            _telemetryMock.Verify(t => t.TrackDependency(
                "Cache",
                "Get",
                testKey,
                true,
                It.IsAny<DateTimeOffset>(),
                It.IsAny<TimeSpan>()),
                Times.Once);

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
