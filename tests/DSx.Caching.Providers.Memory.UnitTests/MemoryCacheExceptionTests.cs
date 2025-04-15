using System;
using System.Text.Json;
using System.Threading;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Validation;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    /// <summary>
    /// Test suite completa per il MemoryCacheProvider
    /// </summary>
    public class MemoryCacheProviderTests
    {
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private readonly Mock<ILogger<MemoryCacheProvider>> _loggerMock = new();
        private readonly ICacheKeyValidator _validator = new CacheKeyValidator();

        /// <summary>
        /// Verifica che SetAsync memorizzi correttamente valori con chiavi valide
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldStoreValue_WithValidKey()
        {
            // Arrange
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            var result = await provider.SetAsync("valid_key", "test_value");

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            _cache.Get<string>("valid_key").Should().Be("test_value");
        }

        /// <summary>
        /// Verifica il fallimento con chiavi non conformi
        /// </summary>
        [Theory]
        [InlineData("invalid key!")]
        [InlineData("")]
        public async Task SetAsync_ShouldFail_WithInvalidKey(string key)
        {
            // Arrange
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            var result = await provider.SetAsync(key, "value");

            // Assert
            result.Status.Should().Be(CacheOperationStatus.ValidationError);
        }

        /// <summary>
        /// Verifica il recupero di valori esistenti
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnValue_WhenKeyExists()
        {
            // Arrange
            _cache.Set("existing_key", "test_value");
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            var result = await provider.GetAsync<string>("existing_key");

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
            result.Value.Should().Be("test_value");
        }

        /// <summary>
        /// Verifica lo svuotamento completo della cache
        /// </summary>
        [Fact]
        public async Task ClearAllAsync_ShouldRemoveAllItems()
        {
            // Arrange
            _cache.Set("key1", "value1");
            _cache.Set("key2", "value2");
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            await provider.ClearAllAsync();

            // Assert
            _cache.Count.Should().Be(0);
        }

        /// <summary>
        /// Verifica la rimozione mirata di una chiave
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ShouldDeleteSpecificKey()
        {
            // Arrange
            _cache.Set("to_remove", "value");
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            await provider.RemoveAsync("to_remove");

            // Assert
            _cache.TryGetValue("to_remove", out _).Should().BeFalse();
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_ForExistingKey()
        {
            // Arrange
            _cache.Set("existing", "value");
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            var result = await provider.ExistsAsync("existing");

            // Assert
            result.Status.Should().Be(CacheOperationStatus.Success);
        }

        /// <summary>
        /// Verifica l'applicazione delle policy di scadenza
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldApplyExpiration_WhenOptionsProvided()
        {
            // Arrange
            var options = new CacheEntryOptions
            {
                AbsoluteExpiration = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act
            await provider.SetAsync("key", "value", options);
            var entry = _cache.Get("key");

            // Assert
            entry.Should().NotBeNull();
        }

        /// <summary>
        /// Verifica la gestione della cancellazione delle operazioni
        /// </summary>
        [Fact]
        public async Task Operations_ShouldCancel_WhenTokenRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var provider = new MemoryCacheProvider(_cache, _loggerMock.Object, _validator);

            // Act/Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                provider.SetAsync("key", "value", cancellationToken: cts.Token));
        }

        /// <summary>
        /// Verifica il ciclo completo di serializzazione/deserializzazione
        /// </summary>
        [Fact]
        public void Serialization_ShouldWork_WithSystemTextJson()
        {
            // Arrange
            var original = new MemoryCacheException(
                "Test error",
                "Technical details",
                new InvalidOperationException("Inner error"));

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new MemoryCacheExceptionConverter() }
            };

            // Act
            var json = JsonSerializer.Serialize(original, options);
            var deserialized = JsonSerializer.Deserialize<MemoryCacheException>(json, options)!;

            // Assert
            deserialized.Message.Should().Be("Test error");
            deserialized.TechnicalDetails.Should().Be("Technical details");
            deserialized.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Be("Inner error");
        }
    }
}