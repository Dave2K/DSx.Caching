using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Servizio per il monitoraggio dello stato dei componenti
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// Verifica lo stato del servizio
        /// </summary>
        /// <returns>Stato di salute del componente</returns>
        HealthStatus CheckHealth();

        /// <summary>
        /// Ottiene metriche operative
        /// </summary>
        IDictionary<string, object> GetMetrics();
    }
}
