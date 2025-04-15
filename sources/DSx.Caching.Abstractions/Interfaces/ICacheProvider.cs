using DSx.Caching.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni base per un provider di cache
    /// </summary>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Evento scatenato prima dell'esecuzione di un'operazione
        /// </summary>
        event EventHandler<CacheEventArgs> BeforeOperation;

        /// <summary>
        /// Evento scatenato dopo il completamento di un'operazione
        /// </summary>
        event EventHandler<CacheEventArgs> AfterOperation;

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare</typeparam>
        /// <param name="key">Chiave identificativa dell'elemento</param>
        /// <param name="options">Opzioni aggiuntive per l'operazione</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione</param>
        /// <returns>
        /// Risultato dell'operazione contenente il valore recuperato
        /// </returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare</typeparam>
        /// <param name="key">Chiave identificativa dell'elemento</param>
        /// <param name="value">Valore da memorizzare</param>
        /// <param name="options">Opzioni di memorizzazione</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione</param>
        /// <returns>
        /// Risultato dell'operazione di memorizzazione
        /// </returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        // Altri metodi con commenti XML...
    }

    /// <summary>
    /// Argomenti degli eventi delle operazioni di cache
    /// </summary>
    public sealed class CacheEventArgs(string key, CacheOperationType operationType) : EventArgs
    {
        /// <summary>
        /// Chiave dell'elemento coinvolto nell'operazione
        /// </summary>
        /// <value>Stringa identificativa della chiave</value>
        public string Key { get; } = key;

        /// <summary>
        /// Tipo di operazione eseguita
        /// </summary>
        /// <value>Valore dell'enum <see cref="CacheOperationType"/></value>
        public CacheOperationType OperationType { get; } = operationType;
    }

    /// <summary>
    /// Tipologie di operazioni supportate sulla cache
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary>
        /// Operazione di recupero
        /// </summary>
        Get,

        /// <summary>
        /// Operazione di memorizzazione
        /// </summary>
        Set,

        // Altri valori con commenti...
    }
}