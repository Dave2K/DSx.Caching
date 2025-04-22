using DSx.Caching.Abstractions.Events;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Events
{
    /// <summary>
    /// Test per verificare il comportamento della classe OperationDeferredEventArgs
    /// </summary>
    public class OperationDeferredEventArgsTests
    {
        /// <summary>
        /// Verifica la corretta inizializzazione degli argomenti
        /// </summary>
        [Fact]
        public void OperationDeferredEventArgs_DovrebbeInizializzareCorrettamente()
        {
            // Arrange
            const string testKey = "test_key";
            const string testReason = "Congestione";

            // Act
            var args = new OperationDeferredEventArgs(testKey, testReason);

            // Assert
            Assert.Equal(testKey, args.Key);
            Assert.Equal(testReason, args.Reason);
        }
    }
}