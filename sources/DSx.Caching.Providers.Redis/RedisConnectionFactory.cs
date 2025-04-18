// DSx.Caching.Providers.Redis/RedisConnectionFactory.cs
using StackExchange.Redis;
using DSx.Caching.SharedKernel.Interfaces;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Implementazione Redis-specifica della connection factory
    /// </summary>
    public class RedisConnectionFactory : IConnectionFactory<IConnectionMultiplexer>
    {
        /// <summary>
        /// Crea una connessione Redis usando la configurazione
        /// </summary>
        /// <param name="configuration">Stringa di configurazione Redis</param>
        public IConnectionMultiplexer CreateConnection(object configuration)
        {
            return ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse((string)configuration)
            );
        }
    }
}