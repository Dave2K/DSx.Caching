using StackExchange.Redis;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Factory per la creazione di connessioni Redis ConnectionMultiplexer
    /// </summary>
    public class RedisConnectionMultiplexerFactory
    {
        /// <summary>
        /// Crea una nuova connessione Redis utilizzando la stringa di configurazione
        /// </summary>
        /// <param name="configurationString">Stringa di configurazione Redis</param>
        /// <returns>Istanza di ConnectionMultiplexer</returns>
        public static IConnectionMultiplexer CreateConnection(string configurationString)
        {
            return ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse(configurationString)
            );
        }
    }
}