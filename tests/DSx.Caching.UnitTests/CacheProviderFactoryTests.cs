using DSx.Caching;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Validators;
using DSx.Caching.Core.Validators;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.UnitTests
{
    /// <summary>
    /// Classe di test per verificare il corretto funzionamento di RedisCacheProvider
    /// </summary>
    public class RedisCacheProviderTests : IDisposable
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ILogger<RedisCacheProvider>> _mockLogger;
        private readonly ICacheKeyValidator _keyValidator;
        private readonly IOptions<JsonSerializerOptions> _jsonOptions;
        private readonly RedisCacheProvider _provider;
        private bool _disposed;

        /// <summary>
        /// Costruttore che inizializza l'ambiente di test
        /// </summary>
        public RedisCacheProviderTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<RedisCacheProvider>>();
            _keyValidator = new CacheKeyValidator();
            _jsonOptions = Options.Create(new JsonSerializerOptions());

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _provider = new RedisCacheProvider(
                _mockConnection.Object,
                _mockLogger.Object,
                _keyValidator,
                _jsonOptions);
        }

        /// <summary>
        /// Verifica che GetAsync restituisca Success quando la chiave esiste
        /// </summary>
        /// <returns>Task asincrono</returns>
        [Fact]
        public async Task GetAsync_ConChiaveValida_RestituisceValore()
        {
            // 1. Configurazione dei servizi
            var services = new ServiceCollection();

            // Aggiungi logging
            services.AddLogging();

            // Configurazione Redis mock
            var mockConnection = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            var endpoint = new System.Net.DnsEndPoint("localhost", 6379);

            mockConnection.Setup(c => c.IsConnected).Returns(true);
            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(mockDatabase.Object);
            mockConnection.Setup(c => c.GetEndPoints(false))
                         .Returns([endpoint]);

            // Registra i servizi necessari
            services.AddSingleton<IConnectionMultiplexer>(mockConnection.Object);
            services.AddSingleton<ICacheKeyValidator, CacheKeyValidator>();
            services.AddSingleton<RedisCacheProvider>();
            services.AddSingleton<ICacheProvider>(sp => sp.GetRequiredService<RedisCacheProvider>());

            // Configura il mock per restituire un valore valido
            mockDatabase.Setup(db => db.StringGetAsync("test_key", CommandFlags.None))
                       .ReturnsAsync("\"test_value\"");

            // 2. Build del service provider
            var serviceProvider = services.BuildServiceProvider();

            // 3. Esecuzione del test
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var result = await cacheProvider.GetAsync<string>("test_key");

            // 4. Verifiche
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal("test_value", result.Value);
        }

        /// <summary>
        /// Verifica che SetAsync memorizzi correttamente un valore e restituisca Success
        /// </summary>
        /// <returns>Task asincrono</returns>
        [Fact]
        public async Task SetAsync_ConDatiValidi_MemorizzaValore()
        {
            // Configurazione
            const string testKey = "test_key";
            const string testValue = "test_value";

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
            mockDatabase.Verify(
                db => db.StringSetAsync(
                    testKey,
                    It.Is<RedisValue>(v => v.ToString() == "\"test_value\""), // Valore serializzato con quotes
                    null,                     // expiry
                    false,                    // keepTtl (parametro mancante nel test originale)
                    When.Always,
                    CommandFlags.None
                ),
                Times.Once,
                "Chiamata a StringSetAsync non corretta"
            );
        }

        /// <summary>
        /// Implementazione di IDisposable per la pulizia delle risorse
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