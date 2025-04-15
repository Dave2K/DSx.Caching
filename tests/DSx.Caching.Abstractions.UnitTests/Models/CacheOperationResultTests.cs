using DSx.Caching.Abstractions.Models;
using FluentAssertions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests.Models
{
    /// <summary>
    /// Test per la classe <see cref="CacheOperationResult"/>
    /// </summary>
    public class CacheOperationResultTests
    {
        /// <summary>
        /// Verifica che la proprietà IsSuccess restituisca True solo per lo stato Success
        /// </summary>
        /// <param name="status">Stato da testare</param>
        /// <param name="expected">Valore atteso</param>
        [Theory]
        [InlineData(CacheOperationStatus.Success, true)]
        [InlineData(CacheOperationStatus.NotFound, false)]
        public void IsSuccess_DovrebbeRestituireValoreCorretto(
            CacheOperationStatus status,
            bool expected)
        {
            var risultato = new CacheOperationResult { Status = status };
            risultato.IsSuccess.Should().Be(expected);
        }

        /// <summary>
        /// Verifica che i dettagli vengano memorizzati correttamente
        /// </summary>
        [Fact]
        public void Details_DovrebbeMemorizzareMessaggioErrore()
        {
            const string messaggio = "Errore di connessione";

            var risultato = new CacheOperationResult
            {
                Status = CacheOperationStatus.ConnectionError,
                Details = messaggio
            };

            risultato.Details.Should().Be(messaggio);
        }
    }
}