// File: DSx.Caching.Providers.Redis.UnitTests/RedisCacheProviderTests.cs
using DSx.Caching.SharedKernel.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using Xunit;

namespace DSx.Caching.Providers.Redis.UnitTests
{
    /// <summary>
    /// Contiene i test per il provider Redis
    /// </summary>
    public class RedisCacheProviderTests
    {
        /// <summary>
        /// Verifica la validazione delle chiavi non valide
        /// </summary>
        [Fact]
        public async Task SetAsync_DovrebbeValidareLaChiave()
        {
            // Configura
            var multiplexerMock = new Mock<IConnectionMultiplexer>();
            var keyValidator = new Mock<ICacheKeyValidator>();
            keyValidator.Setup(v => v.Validate("invalid!key"))
                .Throws(new ArgumentException("Chiave non valida"));

            var provider = new RedisCacheProvider(
                multiplexerMock.Object,
                Mock.Of<ILogger<RedisCacheProvider>>(),
                keyValidator.Object,
                new Microsoft.Extensions.Options.OptionsWrapper<System.Text.Json.JsonSerializerOptions>(
                    new System.Text.Json.JsonSerializerOptions()
                )
            );

            // Esegui & Verifica
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await provider.SetAsync("invalid!key", "value")
            );
        }
    }
}