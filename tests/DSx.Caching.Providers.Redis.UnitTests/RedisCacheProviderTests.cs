using DSx.Caching.Abstractions.Configurations;
using DSx.Caching.Abstractions.Factories;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Test unitari per il provider RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Inizializza una nuova istanza dei test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();

            var config = Options.Create(new RedisCacheProviderConfiguration
            {
                ConnectionString = "localhost:6379",
                OperationTimeoutMs = 5000
            });

            var jsonOptions = Options.Create(new JsonSerializerOptions());
            var logger = Mock.Of<ILogger<RedisCacheProvider>>();
            var keyValidator = new CacheKeyValidator();

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            var connectionFactory = new Mock<IConnectionMultiplexerFactory>();
            connectionFactory.Setup(f => f.CreateConnection(It.IsAny<string>()))
                .Returns(_mockConnection.Object);

            _provider = new RedisCacheProvider(
                connectionFactory.Object,
                config,
                logger,
                keyValidator,
                jsonOptions
            );
        }

        /// <summary>
        /// Verifica il corretto recupero di un elemento esistente
        /// </summary>
        [Fact]
        public async Task GetAsync_ConChiaveEsistente_RestituisceValore()
        {
            // Configurazione
            const string testKey = "chiave_valida";
            const string testValue = "\"valore_test\"";

            _mockDatabase.Setup(d => d.StringGetAsync(testKey, CommandFlags.None))
                .ReturnsAsync(testValue);

            // Esecuzione
            var result = await _provider.GetAsync<string>(testKey);

            // Verifica
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal("valore_test", result.Value);
        }

        /// <summary>
        /// Verifica la corretta gestione degli errori di connessione
        /// </summary>
        [Fact]
        public async Task GetAsync_ConErroreDiConnessione_RestituisceErrore()
        {
            // Configurazione
            const string testKey = "chiave_test";
            _mockDatabase.Setup(d => d.StringGetAsync(testKey, CommandFlags.None))
                .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Errore simulato"));

            // Esecuzione
            var result = await _provider.GetAsync<string>(testKey);

            // Verifica
            Assert.Equal(CacheOperationStatus.ConnectionError, result.Status);
        }

        /// <summary>
        /// Rilascia le risorse del test
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _provider.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}