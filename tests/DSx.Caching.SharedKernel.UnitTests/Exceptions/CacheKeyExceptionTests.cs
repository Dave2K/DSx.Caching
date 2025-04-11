using DSx.Caching.SharedKernel.Enums;
using DSx.Caching.SharedKernel.Exceptions;
using System;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Exceptions // Namespace corretto
{
    public class CacheKeyExceptionTests
    {
        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            // Arrange
            const string codice = "KEY_001";
            var livello = LivelloLog.Error;

            // Act
            var ex = new CacheKeyException(
                message: "Test",
                codiceErrore: codice,
                livelloLog: livello,
                innerException: new ArgumentException("Invalid key")
            );

            // Assert
            Assert.Equal(codice, ex.CodiceErrore);
            Assert.Equal(livello, ex.LivelloLog);
            Assert.IsType<ArgumentException>(ex.InnerException);
        }
    }
}