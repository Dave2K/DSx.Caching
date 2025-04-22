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
        /// Acquisisce un lock distribuito.
        /// </summary>
        /// <param name="key">Chiave del lock.</param>
        /// <param name="timeout">Timeout di attesa.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        Task<IDisposable> AcquireLockAsync(
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Invalida una chiave su tutti i nodi.
        /// </summary>
        /// <param name="key">Chiave da invalidare.</param>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        Task InvalidateAcrossNodesAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica lo stato del servizio.
        /// </summary>
        /// <param name="cancellationToken">Token per annullare l'operazione.</param>
        Task<HealthStatus> CheckHealthAsync(
            CancellationToken cancellationToken = default);
    }
}