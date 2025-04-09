using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using DSx.Caching.Abstractions;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Verifica il rispetto del contratto per l'interfaccia ICacheService
    /// </summary>
    public class CacheServiceContractTests
    {
        private readonly Mock<ICacheService> _mockCacheService = new();

        /// <summary>
        /// Verifica che GetAsync sollevi eccezione con chiave nulla
        /// </summary>
        [Fact]
        public async Task GetAsync_ChiaveNulla_GeneraEccezioneArgomentoNullo()
        {
            // Configurazione mock
            _mockCacheService
                .Setup(s => s.GetAsync<string>(null!))
                .ThrowsAsync(new ArgumentNullException("key"));

            // Verifica
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _mockCacheService.Object.GetAsync<string>(null!)
            );
        }

        /// <summary>
        /// Verifica che SetAsync sollevi eccezione con valore nullo
        /// </summary>
        [Fact]
        public async Task SetAsync_ValoreNullo_GeneraEccezioneArgomentoNullo()
        {
            // Configurazione mock  
            _mockCacheService
                .Setup(s => s.SetAsync<string>("chiave_valida", null!))
                .ThrowsAsync(new ArgumentNullException("value"));

            // Verifica
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _mockCacheService.Object.SetAsync<string>("chiave_valida", null!)
            );
        }

        /// <summary>
        /// Verifica che RemoveAsync sollevi eccezione con chiave vuota
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ChiaveVuota_GeneraEccezioneArgomento()
        {
            // Configurazione mock
            _mockCacheService
                .Setup(s => s.RemoveAsync(string.Empty))
                .ThrowsAsync(new ArgumentException("key"));

            // Verifica
            await Assert.ThrowsAsync<ArgumentException>(
                () => _mockCacheService.Object.RemoveAsync(string.Empty)
            );
        }

        /// <summary>
        /// Verifica il comportamento con chiave contenente spazi
        /// </summary>
        [Fact]
        public async Task SetAsync_ChiaveConSpazi_GeneraEccezioneValidazione()
        {
            // Configurazione mock
            _mockCacheService
                .Setup(s => s.SetAsync("chiave non valida", "valore"))
                .ThrowsAsync(new ArgumentException("Formato chiave non valido"));

            // Verifica
            await Assert.ThrowsAsync<ArgumentException>(
                () => _mockCacheService.Object.SetAsync("chiave non valida", "valore")
            );
        }
    }
}