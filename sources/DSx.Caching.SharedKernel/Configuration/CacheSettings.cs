namespace DSx.Caching.SharedKernel.Configuration
{
    /// <summary>
    /// Configurazione base per tutti i provider di cache
    /// </summary>
    public abstract class CacheSettings
    {
        /// <summary>
        /// Timeout operazioni in millisecondi
        /// </summary>
        public int TimeoutOperazioni { get; set; } = 5000;

        /// <summary>
        /// Abilita la crittografia dei valori
        /// </summary>
        public bool CrittografiaAbilitata { get; set; } = true;
    }
}