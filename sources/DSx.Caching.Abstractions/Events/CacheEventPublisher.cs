using Microsoft.Extensions.Logging;
using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Pubblicatore centralizzato di eventi della cache.
    /// </summary>
    public class CacheEventPublisher
    {
        private readonly ILogger<CacheEventPublisher> _logger;

        /// <summary>
        /// Evento generato prima di un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento generato dopo un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Inizializza una nuova istanza del pubblicatore di eventi.
        /// </summary>
        /// <param name="logger">Logger per tracciamento eventi.</param>
        public CacheEventPublisher(ILogger<CacheEventPublisher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Notifica l'inizio di un'operazione.
        /// </summary>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="operationType">Tipo di operazione.</param>
        public void NotifyBeforeOperation(string key, CacheOperationType operationType)
        {
            var args = new CacheEventArgs(key, operationType);
            BeforeOperation?.Invoke(this, args);

            _logger.LogDebug(
                "Inizio operazione {Operation} su chiave {Key}",
                operationType,
                key
            );
        }

        /// <summary>
        /// Notifica il completamento di un'operazione.
        /// </summary>
        /// <param name="key">Chiave dell'elemento.</param>
        /// <param name="operationType">Tipo di operazione.</param>
        /// <param name="success">Esito dell'operazione.</param>
        public void NotifyAfterOperation(string key, CacheOperationType operationType, bool success)
        {
            var args = new CacheEventArgs(key, operationType, success);
            AfterOperation?.Invoke(this, args);

            _logger.LogDebug(
                "Operazione {Operation} completata. Successo: {Success}",
                operationType,
                success
            );
        }

        /// <summary>
        /// Notifica un evento speciale della cache.
        /// </summary>
        /// <param name="eventId">Tipo di evento speciale.</param>
        /// <param name="message">Messaggio descrittivo.</param>
        /// <param name="exception">Eccezione correlata (opzionale).</param>
        public void NotifySpecialEvent(EventId eventId, string message, Exception? exception = null)
        {
            _logger.Log(
                LogLevel.Error,
                eventId,
                message,
                exception,
                (state, ex) => state.ToString()
            );
        }
    }
}