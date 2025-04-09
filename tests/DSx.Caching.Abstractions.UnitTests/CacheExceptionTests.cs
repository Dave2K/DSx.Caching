using DSx.Caching.Abstractions.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Test per la classe <see cref="CacheException"/>
    /// </summary>
    public class CacheExceptionTests
    {
        /// <summary>
        /// Verifica che il costruttore con messaggio imposti correttamente i dettagli tecnici
        /// </summary>
        [Fact]
        public void Costruttore_ConMessaggioValido_ImpostaTechnicalDetailsCorrettamente()
        {
            // Arrange
            const string messaggio = "Errore di test";

            // Act
            var ex = new CacheException(messaggio);

            // Assert
            ex.TechnicalDetails.Should()
                .Contain("Cache Failure: " + messaggio)
                .And.Contain("Exception Type: DSx.Caching.Abstractions.Exceptions.CacheException");
        }

        /// <summary>
        /// Verifica che le eccezioni interne vengano riportate nei dettagli tecnici
        /// </summary>
        [Fact]
        public void Costruttore_ConEccezioneInterna_IncludiDettagliInnerException()
        {
            // Arrange
            var innerEx = new InvalidOperationException("Errore interno");

            // Act
            var exception = new CacheException("Messaggio principale", innerEx);

            // Assert
            exception.TechnicalDetails.Should()
                .Contain("Inner Exception Type: System.InvalidOperationException")
                .And.Contain("Inner Message: Errore interno");
        }

        /// <summary>
        /// Verifica il comportamento con messaggi non validi (null/vuoti/spazi)
        /// </summary>
        /// <param name="messaggioNonValido">Valore non valido da testare</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Costruttore_ConMessaggioVuoto_GeneraArgumentException(string? messaggioNonValido)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CacheException(messaggioNonValido!))
                .ParamName.Should().Be("message");
        }

        /// <summary>
        /// Verifica la presenza dello stack trace quando non ci sono eccezioni interne
        /// </summary>
        [Fact]
        public void TechnicalDetails_SenzaInnerException_MostraStackTrace()
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
        /// Verifica la rimozione degli spazi bianchi nel messaggio
        /// </summary>
        [Fact]
        public void ValidateMessage_TrimmaSpaziBianchi()
        {
            // Arrange
            const string messaggioOriginale = "  Messaggio con spazi  ";

            // Act
            var ex = new CacheException(messaggioOriginale);

            // Assert
            ex.Message.Should().Be("Messaggio con spazi");
        }
    }
}