// File: tests/DSx.Caching.Core.UnitTests/ProviderContractTests.cs
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DSx.Caching.Core.UnitTests
{
    /// <summary>
    /// Test contrattuali base per tutti i provider di cache
    /// </summary>
    public abstract class ProviderContractTests : IDisposable
    {
        /// <summary>
        /// Istanza del provider sotto test
        /// </summary>
        protected ICacheProvider Provider = null!;

        /// <summary>
        /// Metodo astratto per creare un'istanza del provider
        /// </summary>
        /// <returns>Istanza del provider configurata</returns>
        protected abstract ICacheProvider CreateProvider();

        /// <summary>
        /// Metodo per pulire le risorse del provider dopo i test
        /// </summary>
        protected abstract void CleanupProvider();

        /// <summary>
        /// Test per verificare il comportamento base di SetAsync/GetAsync
        /// </summary>
        [Theory]
        [InlineData("test_key", "test_value")]
        public async Task SetAndGet_ValidKey_ReturnsCorrectValue(string key, string value)
        {
            // Arrange
            Provider = CreateProvider();

            // Act
            await Provider.SetAsync(key, value);
            var result = await Provider.GetAsync<string>(key);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, result.Status);
            Assert.Equal(value, result.Value);
        }

        /// <summary>
        /// Test per verificare la gestione di chiavi non esistenti
        /// </summary>
        [Theory]
        [InlineData("non_existing_key")]
        public async Task Get_NonExistingKey_ReturnsNotFound(string key)
        {
            // Arrange
            Provider = CreateProvider();

            // Act
            var result = await Provider.GetAsync<string>(key);

            // Assert
            Assert.Equal(CacheOperationStatus.NotFound, result.Status);
        }

        /// <summary>
        /// Test per verificare la rimozione corretta delle chiavi
        /// </summary>
        [Theory]
        [InlineData("key_to_remove")]
        public async Task Remove_ExistingKey_RemovesSuccessfully(string key)
        {
            // Arrange
            Provider = CreateProvider();
            await Provider.SetAsync(key, "value");

            // Act
            var removeResult = await Provider.RemoveAsync(key);
            var getResult = await Provider.GetAsync<string>(key);

            // Assert
            Assert.Equal(CacheOperationStatus.Success, removeResult.Status);
            Assert.Equal(CacheOperationStatus.NotFound, getResult.Status);
        }

        /// <summary>
        /// Dispose pattern per pulire le risorse
        /// </summary>
        public void Dispose()
        {
            CleanupProvider();
            GC.SuppressFinalize(this);
        }
    }
}