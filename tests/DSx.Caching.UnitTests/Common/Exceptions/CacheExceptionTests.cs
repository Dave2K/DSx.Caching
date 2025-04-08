namespace DSx.Caching.UnitTests.Common.Exceptions
{
    using DSx.Caching.Abstractions.Exceptions;
    using FluentAssertions;
    using Xunit;

    public class CacheExceptionTests
    {
        [Theory]
        [InlineData("Test message")]
        [InlineData("Another error")]
        public void Constructor_WithValidMessage_SetsProperties(string message)
        {
            // Act
            var exception = new CacheException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.TechnicalDetails.Should().Contain(message);
        }

        [Fact]
        public void Constructor_WithEmptyMessage_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CacheException(""));
        }

        [Fact]
        public void Constructor_WithInnerException_WrapsCorrectly()
        {
            // Arrange
            var innerEx = new System.IO.IOException("Disk failure");

            // Act
            var exception = new CacheException("Wrapper error", innerEx);

            // Assert
            exception.InnerException.Should().Be(innerEx);
            exception.TechnicalDetails.Should().Contain("Wrapper error");
        }

        [Fact]
        public void TechnicalDetails_ContainsFullStackTrace()
        {
            // Arrange
            var exception = new CacheException("Test");

            // Act
            var details = exception.TechnicalDetails;

            // Assert
            details.Should().Contain("Stack:")
                .And.Contain(nameof(CacheException));
        }
    }
}