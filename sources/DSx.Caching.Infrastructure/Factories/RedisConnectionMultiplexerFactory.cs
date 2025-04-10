using DSx.Caching.Abstractions.Factories;
using StackExchange.Redis;

namespace DSx.Caching.Infrastructure.Factories
{
    /// <summary>
    /// Implementazione concreta della factory per connessioni Redis
    /// </summary>
    public class RedisConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        /// <inheritdoc/>
        public IConnectionMultiplexer CreateConnection(string configurationString)
        {
            return ConnectionMultiplexer.Connect(configurationString);
        }
    }
}