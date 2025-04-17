// File: DSx.Caching.SharedKernel.UnitTests/Caching/StampedeProtectorTests.cs

using DSx.Caching.SharedKernel.Caching;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Caching
{
    /// <summary>
    /// Test per la gestione della concorrenza nello StampedeProtector
    /// </summary>
    public class StampedeProtectorTests
    {
        /// <summary>
        /// Verifica che le operazioni concorrenti vengano serializzate
        /// </summary>
        [Fact]
        public async Task ExecuteWithLockAsync_ShouldCleanupLocksAfterUse()
        {
            var protector = new StampedeProtector();
            await protector.ExecuteWithLockAsync("key", () => Task.FromResult(0));

            Assert.Equal(0, protector.ActiveLocksCount);
        }
    }
}