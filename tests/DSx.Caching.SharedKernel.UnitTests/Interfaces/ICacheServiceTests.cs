using DSx.Caching.SharedKernel.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Interfaces
{
    /// <summary>
    /// Test per l'interfaccia <see cref="ICacheService"/>.
    /// </summary>
    public class CacheServiceTests
    {
        private readonly Mock<ICacheService> _mockService = new();

        /// <summary>
        /// Verifica che il metodo RemoveAsync completi con successo.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_CompletesSuccessfully()
        {
            // Arrange
            _mockService.Setup(x => x.RemoveAsync("key")).Returns(Task.CompletedTask);

            // Act & Assert
            await _mockService.Object.RemoveAsync("key");
            _mockService.Verify(x => x.RemoveAsync("key"), Times.Once);
        }
    }
}