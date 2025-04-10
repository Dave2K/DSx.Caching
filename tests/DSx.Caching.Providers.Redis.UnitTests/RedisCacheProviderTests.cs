using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Test unitari per la classe RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ICacheKeyValidator> _mockValidator;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore che inizializza l'ambiente di test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockValidator = new Mock<ICacheKeyValidator>();

            // Configurazione base obbligatoria
            _mockConnection.Setup(c => c.IsConnected).Returns(true);
            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                Mock.Of<ILogger<RedisCacheProvider>>(),
                _mockValidator.Object,
                Options.Create(new JsonSerializerOptions())
            );
        }

        /// <summary>
        /// Verifica che GetAsync sollevi un'eccezione quando viene passata una chiave non valida
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveNonValida_RestituisceErroreValidazione()
        {
            // Configurazione
            const string invalidKey = "chiave non valida!";
            var expectedMessage = "Formato chiave non valido";

            _mockValidator.Setup(v => v.Validate(invalidKey))
                         .Throws(new ArgumentException(expectedMessage));

            // Esecuzione
            var result = await _provider.GetAsync<string>(invalidKey);

            // Debug
            Console.WriteLine($"Status: {result.Status}, Details: {result.Details}");

            // Verifiche
            Assert.Equal(CacheOperationStatus.ValidationError, result.Status);
            Assert.Contains(expectedMessage, result.Details);
        }

        /// <summary>
        /// Verifica che GetAsync restituisca Success con il valore corretto per una chiave valida
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveValida_RestituisceValoreCorretto()
        {
            // Configurazione
            const string testKey = "chiave_valida";
            const string testValue = "\"valore_test\""; // Stringa JSON valida

            // Configurazione completa dei mock
            _mockConnection.Setup(c => c.IsConnected).Returns(true);
            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(_mockDatabase.Object);

            _mockDatabase.Setup(db => db.StringGetAsync(testKey, CommandFlags.None))
                       .ReturnsAsync(testValue);

            // Esecuzione con logging
            var result = await _provider.GetAsync<string>(testKey);

            // Debug: stampa il risultato se il test fallisce
            if (result.Status != CacheOperationStatus.Success)
            {
                Console.WriteLine($"Errore: Status={result.Status}, Details={result.Details}");
            }

            // Verifica
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal("valore_test", result.Value);
        }

        /// <summary>
        /// Verifica che GetAsync restituisca NotFound per una chiave inesistente
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveInesistente_RestituisceNotFound()
        {
            // Configurazione
            const string testKey = "chiave_inesistente";

            _mockDatabase.Setup(db => db.StringGetAsync(testKey, CommandFlags.None))
                       .ReturnsAsync(RedisValue.Null);

            // Esecuzione
            var result = await _provider.GetAsync<string>(testKey);

            // Verifica
            Assert.Equal(CacheOperationStatus.NotFound, result.Status);
        }

        /// <summary>
        /// Verifica che SetAsync memorizzi correttamente un valore valido
        /// </summary>
        [Fact]
        public async Task SetAsync_ValoreValido_MemorizzaCorrettamente()
        {
            // Configurazione
            const string testKey = "chiave_test";
            const string testValue = "valore_test";

            var mockConnection = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();

            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(mockDatabase.Object);

            var provider = new RedisCacheProvider(
                mockConnection.Object,
                Mock.Of<ILogger<RedisCacheProvider>>(),
                new CacheKeyValidator(),
                Options.Create(new JsonSerializerOptions())
            );

            // Esecuzione
            await provider.SetAsync(testKey, testValue);

            // Verifica
            mockDatabase.Verify(db => db.StringSetAsync(
                testKey,
                It.Is<RedisValue>(v => v.ToString() == "\"valore_test\""),
                null,
                false,
                When.Always,
                CommandFlags.None),
            Times.Once);
        }

        /// <summary>
        /// Verifica che RemoveAsync elimini correttamente una chiave esistente
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ChiaveEsistente_EliminaCorrettamente()
        {
            // Configurazione
            const string testKey = "chiave_da_rimuovere";

            _mockDatabase.Setup(db => db.KeyDeleteAsync(testKey, CommandFlags.None))
                       .ReturnsAsync(true);

            // Esecuzione
            var result = await _provider.RemoveAsync(testKey);

            // Verifica
            Assert.Equal(CacheOperationStatus.Success, result.Status);
        }

        /// <summary>
        /// Verifica che ClearAllAsync svuoti correttamente tutta la cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_SvuotaCorrettamenteLaCache()
        {
            // Configurazione
            var mockServer = new Mock<IServer>();
            var endpoint = new System.Net.DnsEndPoint("localhost", 6379);

            _mockConnection.Setup(c => c.GetEndPoints(false))
                         .Returns([endpoint]);

            _mockConnection.Setup(c => c.GetServer(endpoint, null))
                         .Returns(mockServer.Object);

            mockServer.Setup(s => s.FlushAllDatabasesAsync(CommandFlags.None))
                     .Returns(Task.CompletedTask);

            // Esecuzione
            var result = await _provider.ClearAllAsync(CancellationToken.None);

            // Verifica
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            mockServer.Verify(s => s.FlushAllDatabasesAsync(CommandFlags.None), Times.Once);
        }

        /// <summary>
        /// Verifica che venga gestito correttamente un errore di connessione
        /// </summary>
        [Fact]
        public async Task GetAsync_ErroreConnessione_RestituisceErrore()
        {
            // Configurazione
            const string testKey = "chiave_test";

            _mockDatabase.Setup(db => db.StringGetAsync(testKey, CommandFlags.None))
                       .ThrowsAsync(new RedisConnectionException(
                           ConnectionFailureType.UnableToResolvePhysicalConnection,
                           "Errore simulato"));

            // Esecuzione
            var result = await _provider.GetAsync<string>(testKey);

            // Verifica
            Assert.Equal(CacheOperationStatus.ConnectionError, result.Status);
        }

        /// <summary>
        /// Verifica che Dispose chiuda correttamente la connessione
        /// </summary>
        [Fact]
        public void Dispose_ChiudeCorrettamenteLaConnessione()
        {
            // Esecuzione
            _provider.Dispose();

            // Verifica
            _mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        /// <summary>
        /// Implementazione di IDisposable per il cleanup dei test
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _provider?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}