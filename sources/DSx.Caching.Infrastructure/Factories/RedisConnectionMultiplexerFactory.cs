using StackExchange.Redis;
using DSx.Caching.SharedKernel.Interfaces;

namespace DSx.Caching.Infrastructure.Factories
{
    /// <summary>
    /// Factory per la creazione di connessioni Redis ConnectionMultiplexer
    /// </summary>
    public class RedisConnectionMultiplexerFactory : IConnectionFactory<IConnectionMultiplexer>
    {
        /// <summary>
        /// Crea una nuova connessione Redis utilizzando la stringa di configurazione
        /// </summary>
        /// <param name="configuration">Stringa di configurazione Redis</param>
        /// <returns>Istanza di ConnectionMultiplexer</returns>
        public IConnectionMultiplexer CreateConnection(object configuration)
        {
            return ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse((string)configuration)
            );
        }
    }
}
