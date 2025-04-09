using DSx.Caching.Abstractions.Exceptions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions
{
    /// <summary>
    /// Classe base astratta che implementa la logica comune per tutti i provider di cache
    /// </summary>
    /// <remarks>
    /// Fornisce un'implementazione base per la gestione degli errori e il logging
    /// </remarks>
    public abstract class BaseCacheProvider(ILogger logger) : ICacheProvider
    {
        /// <summary>
        /// Logger utilizzato per tracciare le operazioni e gli errori
        /// </summary>
        protected readonly ILogger Logger = logger;

        /// <summary>
        /// Verifica se una chiave esiste nella cache
        /// </summary>
        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> ExistsAsync(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <inheritdoc/>
        public abstract Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove una chiave dalla cache
        /// </summary>
        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente il contenuto della cache
        /// </summary>
        /// <inheritdoc/>
        public abstract Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rilascia le risorse non gestite
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rilascia le risorse non gestite in modo asincrono
        /// </summary>
        public virtual ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Gestisce un errore verificatosi durante un'operazione di cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore di ritorno</typeparam>
        /// <param name="ex">Eccezione verificatasi</param>
        /// <param name="operation">Nome dell'operazione fallita</param>
        /// <param name="key">Chiave coinvolta nell'operazione</param>
        /// <returns>Risultato dell'operazione con lo stato di errore</returns>
        protected CacheOperationResult<T> HandleError<T>(Exception ex, string operation, string key)
        {
            Logger.LogError(ex, "{Operation} failed for key: {Key}", operation, key);
            return new CacheOperationResult<T>
            {
                Status = ex is CacheException ? CacheOperationStatus.ValidationError
                                           : CacheOperationStatus.ConnectionError,
                Details = ex.Message
            };
        }
    }
}