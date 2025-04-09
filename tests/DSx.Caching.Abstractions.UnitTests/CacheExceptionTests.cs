using DSx.Caching.Abstractions.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Contiene i test unitari per la classe <see cref="CacheException"/>.
    /// </summary>
    public class CacheExceptionTests
    {
        /// <summary>
        /// Verifica che il costruttore con messaggio imposti correttamente i dettagli tecnici.
        /// </summary>
        [Fact]
        public void Constructor_WithMessage_ShouldSetTechnicalDetails()
        {
            // Arrange
            const string expectedMessage = "Test message";

            // Act
            var ex = new CacheException(expectedMessage);

            // Assert
            ex.Message.Should().Be(expectedMessage);
            ex.TechnicalDetails.Should()
                .Contain("Cache Failure: " + expectedMessage)
                .And.Contain("Exception Type: DSx.Caching.Abstractions.Exceptions.CacheException");
        }

        /// <summary>
        /// Verifica che il costruttore con eccezione interna includa i dettagli dell'eccezione originale.
        /// </summary>
        [Fact]
        public void Constructor_WithMessageAndInnerException_ShouldIncludeInnerDetails()
        {
            // Arrange
            const string expectedMessage = "Test message";
            const string innerMessage = "Errore interno";
            InvalidOperationException innerEx;

            try
            {
                throw new InvalidOperationException(innerMessage);
            }
            catch (InvalidOperationException ex)
            {
                innerEx = ex;
            }

            // Act
            var exception = new CacheException(expectedMessage, innerEx);

            // Assert
            exception.TechnicalDetails.Should()
                .Contain("Inner Exception Type: System.InvalidOperationException")
                .And.Contain($"Inner Message: {innerMessage}")
                .And.Contain("Inner Stack Trace:");
        }

        /// <summary>
        /// Verifica che venga lanciata un'eccezione con messaggio vuoto.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyMessage_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CacheException(""))
                .ParamName.Should().Be("message");
        }

        /// <summary>
        /// Verifica che venga lanciata un'eccezione con messaggio contenente solo spazi bianchi.
        /// </summary>
        [Fact]
        public void Constructor_WithWhitespaceMessage_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CacheException("   "))
                .ParamName.Should().Be("message");
        }

        /// <summary>
        /// Verifica che i dettagli tecnici mostrino lo stack trace quando non è presente un'eccezione interna.
        /// </summary>
        [Fact]
        public void TechnicalDetails_WithoutInnerException_ShouldShowStackTrace()
        {
            // Arrange
            var ex = new CacheException("Test");

            // Act
            var details = ex.TechnicalDetails;

            // Assert
            details.Should()
                .Contain("Stack Trace:")
                .And.Contain("DSx.Caching.Abstractions.Exceptions.CacheException");
        }

        /// <summary>
        /// Verifica che i messaggi vengano trimmati dagli spazi bianchi.
        /// </summary>
        [Fact]
        public void ValidateMessage_ShouldTrimWhitespace()
        {
            // Arrange
            const string messageWithWhitespace = "  Test message  ";

            // Act
            var ex = new CacheException(messageWithWhitespace);

            // Assert
            ex.Message.Should().Be("Test message");
        }
    }
}