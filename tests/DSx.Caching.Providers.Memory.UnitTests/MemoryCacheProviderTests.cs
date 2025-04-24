using DSx.Caching.Abstractions.Models;
using DSx.Caching.Providers.Memory;
using DSx.Caching.SharedKernel.Validation;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Contiene i test unitari per il provider di cache in memoria
    /// </summary>
    public class MemoryCacheProviderTests
    {
        private readonly MemoryCacheProvider _provider;
        private readonly Mock<ILogger<MemoryCacheProvider>> _mockLogger = new();

        /// <summary>
        /// Inizializza una nuova istanza del provider di cache per i test
        /// </summary>
        public MemoryCacheProviderTests()
        {
            _provider = new MemoryCacheProvider(
                new MemoryCache(new MemoryCacheOptions()),
                _mockLogger.Object,
                new CacheKeyValidator(),
                TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Verifica che le operazioni SetAsync e GetAsync funzionino correttamente
        /// </summary>
        [Fact]
        public async Task SetAsync_e_GetAsync_DovrebberoFunzionareCorrettamente()
        {
            // Arrange
            const string key = "test_key";
            const string value = "test_value";

            // Act
            await _provider.SetAsync(key, value);
            var result = await _provider.GetAsync<string>(key);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be(value);
        }

        /// <summary>
        /// Verifica che il GetAsync restituisca NotFound per una chiave inesistente
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveInesistente_DovrebbeRestituireNotFound()
        {
            // Act
            var result = await _provider.GetAsync<string>("key_inesistente");

            // Assert
            result.Status.Should().Be(CacheOperationStatus.NotFound);
        }

        /// <summary>
        /// Verifica che il RemoveAsync elimini correttamente una chiave esistente
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ChiaveEsistente_DovrebbeEliminare()
        {
            // Arrange
            const string key = "key_to_remove";
            await _provider.SetAsync(key, "value");

            // Act
            var result = await _provider.RemoveAsync(key);

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
        }

        /// <summary>
        /// Verifica che il ClearAllAsync svuoti completamente la cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_DovrebbeSvuotareCache()
        {
            // Arrange
            await _provider.SetAsync("key1", "value1");
            await _provider.SetAsync("key2", "value2");

            // Act
            var result = await _provider.ClearAllAsync();

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
        }

        /// <summary>
        /// Verifica che il GetDescriptorAsync restituisca metadati corretti per una chiave
        /// </summary>
        [Fact]
        public async Task GetDescriptorAsync_DovrebbeRestituireMetadatiCorretti()
        {
            // Arrange
            const string key = "key_with_metadata";
            await _provider.SetAsync(key, "value");

            // Act
            var descriptor = await _provider.GetDescriptorAsync(key);

            // Assert
            descriptor.Should().NotBeNull();
            descriptor!.Key.Should().Be(key);
            descriptor.ReadCount.Should().Be(0); // Non è stato letto ancora
        }
    }
}
