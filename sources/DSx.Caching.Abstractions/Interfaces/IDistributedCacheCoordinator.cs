using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Gestisce la coordinazione distribuita per operazioni su cache
    /// </summary>
    public interface IDistributedCacheCoordinator
    {
        /// <summary>
        /// Acquisisce un lock distribuito
        /// </summary>
        /// <param name="key">Chiave su cui acquisire il lock</param>
        /// <param name="timeout">Timeout di attesa</param>
        /// <returns>Disposable per rilasciare il lock</returns>
        Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout);

        /// <summary>
        /// Invalida una chiave su tutti i nodi
        /// </summary>
        /// <param name="key">Chiave da invalidare</param>
        /// <returns>Task asincrono</returns>
        Task InvalidateAcrossNodesAsync(string key);

        /// <summary>
        /// Verifica lo stato di salute del coordinatore
        /// </summary>
        /// <returns>Stato di salute corrente</returns>
        Task<HealthStatus> CheckHealthAsync();
    }
}