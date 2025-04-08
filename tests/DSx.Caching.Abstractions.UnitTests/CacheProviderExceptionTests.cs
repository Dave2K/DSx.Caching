using DSx.Caching.Abstractions.Exceptions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Contiene i test per la classe <see cref="CacheProviderException"/>
    /// </summary>
    public class CacheProviderExceptionTests
    {
        /// <summary>
        /// Verifica che il costruttore con messaggio ed eccezione interna
        /// inizializza correttamente le proprietà dell'eccezione
        /// </summary>
        [Fact]
        public void Constructor_WithMessageAndInnerException_InitializesCorrectly()
        {
            // Arrange
            var innerEx = new InvalidOperationException();

            // Act
            var ex = new CacheProviderException("test", innerEx);

            // Assert
            Assert.Equal("test", ex.Message);
            Assert.Same(innerEx, ex.InnerException);
        }
    }
}