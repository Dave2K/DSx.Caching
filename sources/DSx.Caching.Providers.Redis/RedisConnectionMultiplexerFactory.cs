using StackExchange.Redis;
using DSx.Caching.Abstractions.Factories;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Factory per la creazione di connessioni Redis
    /// </summary>
    public class RedisConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Crea una nuova connessione Redis
        /// </summary>
        /// <param name="configurationString">Stringa di configurazione Redis</param>
        /// <returns>Istanza di IConnectionMultiplexer</returns>
        public IConnectionMultiplexer CreateConnection(string configurationString)
        {
            return ConnectionMultiplexer.Connect(configurationString);
        }
    }
}