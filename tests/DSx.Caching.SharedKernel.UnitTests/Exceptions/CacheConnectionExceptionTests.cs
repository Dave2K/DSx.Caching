using DSx.Caching.SharedKernel.Enums;
using DSx.Caching.SharedKernel.Exceptions;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.SharedKernel.UnitTests.Exceptions
{
    /// <summary>
    /// Test suite per la classe <see cref="CacheConnectionException"/>
    /// </summary>
    public class CacheConnectionExceptionTests
    {
        /// <summary>
        /// Verifica che il costruttore imposti correttamente tutte le proprietà
        /// </summary>
        [Fact]
        public void Costruttore_DovrebbeImpostareTutteLeProprietà()
        {
            // Arrange
            const string provider = "Redis";
            const string codice = "CONN_001";
            var innerEx = new TimeoutException();

            // Act
            var ex = new CacheConnectionException(
                message: "Connection failed",
                nomeProvider: provider,
                codiceErrore: codice,
                livelloLog: LivelloLog.Critical,
                innerException: innerEx);

            // Assert
            ex.NomeProvider.Should().Be(provider);
            ex.CodiceErrore.Should().Be(codice);
            ex.LivelloLog.Should().Be(LivelloLog.Critical);
            ex.InnerException.Should().BeSameAs(innerEx);
        }

        /// <summary>
        /// Verifica che il metodo ToString() includa tutte le informazioni rilevanti
        /// </summary>
        [Fact]
        public void ToString_DovrebbeContenereInformazioniComplete()
        {
            // Arrange
            var innerEx = new ArgumentException("Invalid parameter");
            var ex = new CacheConnectionException(
                "Test error",
                "MemoryCache",
                "CONN_002",
                LivelloLog.Error,
                innerEx);

            // Act
            var result = ex.ToString();

            // Assert
            result.Should()
                .Contain("Test error")
                .And.Contain("MemoryCache")
                .And.Contain("CONN_002")
                .And.Contain("ArgumentException")
                .And.Contain("Livello: Error")
                .And.Contain("Provider: MemoryCache")
                .And.Contain("Codice: CONN_002");
        }
    }
}