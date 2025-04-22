using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Configurazione per i lock distribuiti (Funzionalità Enterprise)
    /// </summary>
    public class DistributedLockOptions
    {
        /// <summary>
        /// Timeout per l'acquisizione del lock
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Numero massimo di tentativi di acquisizione
        /// </summary>
        public int RetryAttempts { get; set; } = 3;

        /// <summary>
        /// Intervallo tra i tentativi
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(500);
    }
}