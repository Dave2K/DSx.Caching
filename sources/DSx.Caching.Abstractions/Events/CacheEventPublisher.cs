using Microsoft.Extensions.Logging;
using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Gestisce la pubblicazione degli eventi relativi alle operazioni sulla cache.
    /// Fornisce notifiche prima e dopo ogni operazione, oltre a gestire eventi speciali.
    /// </summary>
    public class CacheEventPublisher
    {
        private readonly ILogger<CacheEventPublisher> _logger;

        /// <summary>
        /// Evento sollevato prima dell'esecuzione di un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo il completamento di un'operazione sulla cache.
        /// </summary>
        public event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheEventPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger per la registrazione degli eventi.</param>
        public CacheEventPublisher(ILogger<CacheEventPublisher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Notifica l'inizio di un'operazione sulla cache.
        /// </summary>
        /// <param name="key">Chiave dell'elemento coinvolto nell'operazione.</param>
        /// <param name="operationType">Tipo di operazione eseguita.</param>
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
        /// Notifica il completamento di un'operazione sulla cache.
        /// </summary>
        /// <param name="key">Chiave dell'elemento coinvolto nell'operazione.</param>
        /// <param name="operationType">Tipo di operazione eseguita.</param>
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
        /// Registra un evento speciale con livello di gravità Error.
        /// </summary>
        /// <param name="eventId">Identificativo univoco dell'evento.</param>
        /// <param name="message">Messaggio descrittivo dell'evento.</param>
        /// <param name="exception">Eccezione correlata all'evento (opzionale).</param>
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
