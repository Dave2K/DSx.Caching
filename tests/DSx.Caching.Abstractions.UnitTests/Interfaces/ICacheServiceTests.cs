using DSx.Caching.Abstractions.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Interfaces
{
    /// <summary>
    /// Test per <see cref="ICacheService"/>
    /// </summary>
    public class CacheServiceTests
    {
        private readonly Mock<ICacheService> _mockService = new();

        /// <summary>
        /// Verifica che RemoveAsync completi l'operazione
        /// </summary>
        [Fact]
        public async Task RemoveAsync_CompletesSuccessfully()
        {
            // Arrange
            _mockService
                .Setup(x => x.RemoveAsync("key"))
                .Returns(Task.CompletedTask);

            // Act & Assert (no exceptions)
            await _mockService.Object.RemoveAsync("key");
        }

        /// <summary>
        /// Verifica che SetAsync gestisca valori null
        /// </summary>
        [Fact]
        public async Task SetAsync_HandlesNullValues()
        {
            // Arrange
            _mockService
                .Setup(x => x.SetAsync<string?>("key", null)) // Aggiungi '?' a string
                .Returns(Task.CompletedTask);

            // Act & Assert (no exceptions)
            await _mockService.Object.SetAsync("key", (string?)null); // Aggiungi '?' al cast
        }
    }
}