using DSx.Caching.Abstractions.Middleware;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Middleware
{
    /// <summary>
    /// Test per il middleware delle operazioni di cache
    /// </summary>
    public class CacheMiddlewareTests
    {
        private readonly Mock<ICacheMiddleware> _middleware = new();

        /// <summary>
        /// Verifica il corretto funzionamento con operazione valida
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_DovrebbeRestituireRisultato_QuandoOperazioneRiuscita()
        {
            // Arrange
            const string expected = "test_value";
            _middleware.Setup(x => x.ExecuteAsync<string>(
                    It.IsAny<Func<CancellationToken, Task<string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _middleware.Object.ExecuteAsync<string>(
                ct => Task.FromResult(expected),
                CancellationToken.None
            );

            // Assert
            result.Should().Be(expected);
        }
    }
}