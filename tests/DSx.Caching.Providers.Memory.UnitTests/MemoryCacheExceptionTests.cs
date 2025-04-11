using DSx.Caching.Providers.Memory;
using System;
using Xunit;

namespace DSx.Caching.Providers.Memory.UnitTests
{
    public class MemoryCacheExceptionTests
    {
        [Fact]
        public void Should_Contain_TechnicalDetails()
        {
            // Arrange
            var innerEx = new Exception("Inner");
            const string details = "ERR-001";

            // Act
            var ex = new MemoryCacheException(
                message: "Error",
                technicalDetails: details,
                inner: innerEx
            );

            // Assert
            Assert.Equal(details, ex.TechnicalDetails);
            Assert.Same(innerEx, ex.InnerException);
        }
    }
}