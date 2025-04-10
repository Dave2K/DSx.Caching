namespace DSx.Caching.Abstractions.Configurations
{
    /// <summary>
    /// Configurazione specifica per il provider Redis
    /// </summary>
    public class RedisCacheProviderConfiguration
    {
        /// <summary>
        /// Stringa di connessione a Redis (es: "localhost:6379")
        /// </summary>
        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>
        /// Timeout per le operazioni in millisecondi
        /// </summary>
        public int OperationTimeoutMs { get; set; } = 5000;
    }
}