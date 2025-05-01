using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Fornisce un meccanismo per gestire gli errori temporanei tramite un circuito che si apre dopo un certo numero di fallimenti.
    /// </summary>
    public interface ICacheCircuitBreaker
    {
        /// <summary>
        /// Esegue un'operazione con resilienza, applicando il pattern Circuit Breaker.
        /// </summary>
        /// <typeparam name="T">Tipo del risultato dell'operazione.</typeparam>
        /// <param name="action">Operazione da eseguire.</param>
        /// <param name="fallbackAction">Azione di fallback da eseguire se il circuito è aperto.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Il risultato dell'operazione o del fallback.</returns>
        Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Func<CancellationToken, Task<T>> fallbackAction,
            CancellationToken cancellationToken = default);
    }
}
