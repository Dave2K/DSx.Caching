using DSx.Caching.SharedKernel.Enums;
using DSx.Caching.SharedKernel.Exceptions;
using System;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Exceptions
{
    public class CacheConnectionExceptionTests
    {
        [Fact]
        public void Should_Contain_Correct_Properties()
        {
            // Arrange
            var inner = new Exception("Test");
            const string provider = "Redis";
            const string code = "CONN_001";

            // Act
            var ex = new CacheConnectionException(
                "Connection failed",
                provider,
                code,
                LivelloLog.Critical, // Usato valore corretto
                inner
            );

            // Assert
            Assert.Equal(provider, ex.NomeProvider);
            Assert.Equal(code, ex.CodiceErrore);
            Assert.Equal(LivelloLog.Critical, ex.LivelloLog);
        }
    }
}