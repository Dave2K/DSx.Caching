using System;
using System.Runtime.Serialization;
using DSx.Caching.SharedKernel.Exceptions;
using Xunit;

namespace DSx.Caching.Abstractions.UnitTests
{
    /// <summary>
    /// Test suite per la gestione delle eccezioni nella cache
    /// </summary>
    public class CacheExceptionTests
    {
        /// <summary>
        /// Verifica la corretta inizializzazione con messaggio ed eccezione interna
        /// </summary>
        [Fact(DisplayName = "CTOR con messaggio e inner exception")]
        public void CostruttoreConMessaggioEInner_InizializzaCorrettamente()
        {
            // Arrange
            var innerException = new InvalidOperationException();
            const string messaggio = "Errore critico di cache";

            // Act
            var exception = new CacheException(messaggio, innerException);

            // Assert
            Assert.Equal(messaggio, exception.Message);
            Assert.Same(innerException, exception.InnerException);
        }

        /// <summary>
        /// Verifica la corretta serializzazione/deserializzazione
        /// </summary>
        [Fact(DisplayName = "Serializzazione/Deserializzazione legacy")]
        public void SerializzazioneDeserializzazione_Corretta()
        {
            // Arrange
            var originalException = new CacheException("Test Error");

            // Act
#pragma warning disable SYSLIB0050
            var info = new SerializationInfo(typeof(CacheException), new FormatterConverter());
#pragma warning restore SYSLIB0050

            originalException.GetObjectData(info, new StreamingContext());
            var deserializedException = new CacheException(info, new StreamingContext());

            // Assert
            Assert.Equal(originalException.Message, deserializedException.Message);
        }
    }
}