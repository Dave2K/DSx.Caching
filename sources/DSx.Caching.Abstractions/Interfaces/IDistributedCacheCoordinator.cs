using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce un coordinatore per lock distribuiti e health check.
    /// </summary>
    public interface IDistributedCacheCoordinator
    {
        /// <summary>
        /// Acquisisce un lock distribuito con timeout specificato.
        /// </summary>
        /// <param name="key">Chiave univoca del lock.</param>
        /// <param name="timeout">Durata massima di attesa per l'acquisizione.</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Disposable per rilasciare il lock.</returns>
        /// <exception cref="ArgumentNullException">Se key è null o vuota.</exception>
        /// <exception cref="TimeoutException">Se il timeout scade.</exception>
        Task<IDisposable> AcquireLockAsync(
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster.
        /// </summary>
        /// <param name="key">Chiave da invalidare.</param>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Task asincrono.</returns>
        /// <exception cref="ArgumentNullException">Se key è null o vuota.</exception>
        Task InvalidateAcrossNodesAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica lo stato di salute del servizio.
        /// </summary>
        /// <param name="cancellationToken">Token per annullamento operazione.</param>
        /// <returns>Stato di salute corrente.</returns>
        Task<HealthStatus> CheckHealthAsync(
            CancellationToken cancellationToken = default);
    }
}