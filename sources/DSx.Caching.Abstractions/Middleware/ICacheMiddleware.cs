using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Middleware
{
    /// <summary>
    /// Fornisce un meccanismo per intercettare e gestire le operazioni di cache
    /// </summary>
    public interface ICacheMiddleware
    {
        /// <summary>
        /// Esegue un'operazione di cache con gestione degli errori e tracciamento
        /// </summary>
        /// <typeparam name="T">Tipo del valore restituito</typeparam>
        /// <param name="operation">Operazione da eseguire</param>
        /// <param name="ct">Token per l'annullamento</param>
        /// <returns>Risultato dell'operazione</returns>
        Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken ct);
    }
}