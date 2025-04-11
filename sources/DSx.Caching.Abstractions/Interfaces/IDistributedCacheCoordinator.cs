using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Coordina operazioni avanzate in ambienti distribuiti con cache condivisa
    /// </summary>
    public interface IDistributedCacheCoordinator
    {
        /// <summary>
        /// Acquisisce un lock distribuito per una chiave specifica
        /// </summary>
        /// <param name="key">Chiave su cui acquisire il lock</param>
        /// <param name="timeout">Durata massima per l'acquisizione</param>
        /// <returns>Disposable per rilasciare il lock</returns>
        Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout);

        /// <summary>
        /// Invalida una chiave su tutti i nodi del cluster
        /// </summary>
        /// <param name="key">Chiave da invalidare</param>
        Task InvalidateAcrossNodesAsync(string key);

        /// <summary>
        /// Sincronizza lo stato della cache tra tutti i nodi
        /// </summary>
        Task SyncStateAsync();

        /// <summary>
        /// Verifica lo stato di salute del coordinatore
        /// </summary>
        /// <returns>Stato di salute corrente</returns>
        Task<HealthStatus> CheckHealthAsync();
    }
}