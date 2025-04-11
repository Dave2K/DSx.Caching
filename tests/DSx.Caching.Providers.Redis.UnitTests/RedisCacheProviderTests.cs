using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Text.Json;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    public class RedisCacheProviderTests
    {
        private readonly Mock<IConnectionMultiplexer> _multiplexerMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly ILogger<RedisCacheProvider> _logger;
        private readonly RedisCacheProvider _provider;

        public RedisCacheProviderTests()
        {
            // Configurazione mock Redis
            _multiplexerMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();

            _multiplexerMock.Setup(x => x.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object>()
            )).Returns(_databaseMock.Object);

            // Configurazione logger
            _logger = Mock.Of<ILogger<RedisCacheProvider>>();

            // Inizializzazione provider con parametri corretti
            _provider = new RedisCacheProvider(
                connectionMultiplexer: _multiplexerMock.Object, // Nome parametro corretto
                logger: _logger,
                keyValidator: new Mock<ICacheKeyValidator>().Object, // Usa interfaccia
                options: Options.Create(new JsonSerializerOptions())
            );
        }

        [Fact]
        public async Task SetAsync_ShouldValidateKey()
        {
            // Arrange
            var invalidKey = "invalid key!";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _provider.SetAsync(invalidKey, "value", null)
            );
        }
    }
}