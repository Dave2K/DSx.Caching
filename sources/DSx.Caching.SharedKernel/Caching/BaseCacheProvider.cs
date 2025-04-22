using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Fornisce un'implementazione base astratta per i provider di cache
    /// </summary>
    public abstract class BaseCacheProvider : ICacheProvider, IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Logger per la registrazione delle attività
        /// </summary>
        protected readonly ILogger Logger;

        private bool _disposed;

        /// <summary>
        /// Evento sollevato prima dell'esecuzione di qualsiasi operazione sulla cache
        /// </summary>
        public abstract event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo il completamento di qualsiasi operazione sulla cache
        /// </summary>
        public abstract event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Evento sollevato quando un'operazione viene posticipata per motivi specifici
        /// </summary>
        public abstract event EventHandler<OperationDeferredEventArgs>? OperationDeferred;

        /// <summary>
        /// Inizializza una nuova istanza della classe base per i provider di cache
        /// </summary>
        /// <param name="logger">Istanza del logger per la registrazione delle attività</param>
        /// <exception cref="ArgumentNullException">Generato quando il logger è null</exception>
        protected BaseCacheProvider(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache
        /// </summary>
        /// <param name="key">Chiave da verificare</param>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Risultato dell'operazione con stato e dettagli</returns>
        public abstract Task<CacheOperationResult> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache utilizzando la chiave specificata
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave identificativa del valore</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Risultato contenente il valore recuperato o errori</returns>
        public abstract Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene i metadati tecnici di una voce della cache
        /// </summary>
        /// <param name="key">Chiave identificativa della voce</param>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Descrittore tecnico della voce o null se non presente</returns>
        public abstract Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache associandolo a una chiave
        /// </summary>
        /// <typeparam name="T">Tipo generico del valore da memorizzare</typeparam>
        /// <param name="key">Chiave univoca per l'identificazione</param>
        /// <param name="value">Valore da memorizzare nella cache</param>
        /// <param name="options">Opzioni di configurazione per la memorizzazione</param>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Risultato dell'operazione di memorizzazione</returns>
        public abstract Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una voce specifica dalla cache
        /// </summary>
        /// <param name="key">Chiave della voce da rimuovere</param>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Risultato dell'operazione di rimozione</returns>
        public abstract Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente il contenuto della cache
        /// </summary>
        /// <param name="cancellationToken">Token per annullamento operativo</param>
        /// <returns>Risultato dell'operazione di svuotamento</returns>
        public abstract Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica lo stato di disposizione dell'oggetto
        /// </summary>
        /// <exception cref="ObjectDisposedException">Generato se l'oggetto è già stato eliminato</exception>
        protected void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Esegue la disposizione delle risorse gestite
        /// </summary>
        /// <param name="disposing">Indica se la disposizione è esplicita</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Logica di pulizia per risorse gestite
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Implementazione dell'interfaccia IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione dell'interfaccia IAsyncDisposable
        /// </summary>
        public virtual ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}